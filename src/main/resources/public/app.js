const $ = id => document.getElementById(id);
const money = n => 'R' + Number(n).toFixed(2);
const esc = s => String(s ?? '').replace(/[&<>"']/g,
  c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));

let products = [];
let msgTimer;

function msg(text, ok) {
  const m = $('msg');
  m.textContent = text;
  m.className = ok ? 'ok' : 'err';
  clearTimeout(msgTimer);
  msgTimer = setTimeout(() => { m.textContent = ''; }, 6000);
}

async function api(url, opts = {}) {
  const res = await fetch(url, { headers: { 'Content-Type': 'application/json' }, ...opts });
  let data = {};
  try { data = await res.json(); } catch (e) { /* empty body */ }
  if (res.status === 401) showAuth();
  if (!res.ok) throw new Error(data.error || 'Request failed');
  return data;
}
const post = (url, body) => api(url, { method: 'POST', body: JSON.stringify(body || {}) });

/* ---------- views ---------- */
function show(view) {
  ['auth', 'shop', 'orders'].forEach(v => { $('view-' + v).hidden = v !== view; });
  $('nav').hidden = view === 'auth';
}
function showAuth() { show('auth'); }

async function enterShop(user) {
  if (user) $('who').textContent = user.username;
  show('shop');
  await loadProducts();
  renderCart(await api('/api/cart'));
}

/* ---------- products ---------- */
async function loadProducts() {
  products = await api('/api/products');
  renderProducts();
}

function renderProducts() {
  const q = $('search').value.trim().toLowerCase();
  const list = products.filter(p => p.name.toLowerCase().includes(q));
  $('products').innerHTML = list.length ? list.map(p => `
    <div class="product">
      <div><strong>${esc(p.name)}</strong>
        <div class="meta">${money(p.price)} &middot; ${p.stock > 0 ? 'stock: ' + p.stock : 'Out of stock'}</div></div>
      <div>
        <input type="number" id="qty-${p.id}" value="1" min="1" ${p.stock === 0 ? 'disabled' : ''}>
        <button data-add="${p.id}" ${p.stock === 0 ? 'disabled' : ''}>Add</button>
      </div>
    </div>`).join('') : '<p class="meta">No products match.</p>';
}

/* ---------- cart ---------- */
function renderCart(cart) {
  const el = $('cart');
  if (cart.items.length === 0) { el.innerHTML = '<p class="meta">Cart is empty.</p>'; return; }
  el.innerHTML = cart.items.map(i => `
    <div class="row"><span>${esc(i.product.name)} x${i.quantity}</span>
    <span>${money(i.subtotal)}<button class="link" data-remove="${i.product.id}" title="Remove">&times;</button></span></div>`).join('')
    + `<div class="row total"><span>Total</span><span>${money(cart.total)}</span></div>`;
}

async function addToCart(id) {
  try {
    const qty = parseInt($('qty-' + id).value, 10);
    if (!Number.isInteger(qty)) throw new Error('Enter a whole number quantity');
    renderCart(await post('/api/cart', { productId: id, quantity: qty }));
    msg('Added to cart.', true);
  } catch (e) { msg(e.message, false); }
}

async function removeItem(id) {
  try { renderCart(await api('/api/cart/' + id, { method: 'DELETE' })); msg('Removed.', true); }
  catch (e) { msg(e.message, false); }
}

$('products').addEventListener('click', e => {
  const id = e.target.dataset.add;
  if (id) addToCart(Number(id));
});
$('cart').addEventListener('click', e => {
  const id = e.target.dataset.remove;
  if (id) removeItem(Number(id));
});
$('search').addEventListener('input', renderProducts);

$('btn-clear').onclick = async () => {
  try {
    renderCart(await api('/api/cart', { method: 'DELETE' }));
    $('pay-form').hidden = true;
  } catch (e) { msg(e.message, false); }
};

/* ---------- checkout and payment ---------- */
$('btn-checkout').onclick = async () => {
  try {
    const cart = await api('/api/cart');
    if (cart.items.length === 0) throw new Error('Cart is empty');
    $('pay-form').hidden = false;
    $('card').focus();
  } catch (e) { msg(e.message, false); }
};

document.querySelectorAll('input[name=method]').forEach(r => r.addEventListener('change', () => {
  $('card-fields').hidden = document.querySelector('input[name=method]:checked').value !== 'CARD';
}));

$('pay-form').addEventListener('submit', async e => {
  e.preventDefault();
  try {
    const method = document.querySelector('input[name=method]:checked').value;
    const order = await post('/api/checkout', {
      method, cardNumber: $('card').value, expiry: $('exp').value, cvv: $('cvv').value
    });
    $('pay-form').reset();
    $('card-fields').hidden = false;
    $('pay-form').hidden = true;
    msg(`Order #${order.id} placed. Total ${money(order.total)}. Reference ${order.reference}`, true);
    await loadProducts();
    renderCart(await api('/api/cart'));
  } catch (err) { msg(err.message, false); }
});

/* ---------- orders ---------- */
async function loadOrders() {
  const orders = await api('/api/orders');
  show('orders');
  $('orders').innerHTML = orders.length ? orders.slice().reverse().map(o => `
    <div class="order">
      <div class="row"><strong>Order #${o.id}</strong><span class="meta">${esc(o.createdAt.replace('T', ' '))}</span></div>
      ${o.items.map(i => `<div class="row"><span>${esc(i.product.name)} x${i.quantity}</span><span>${money(i.subtotal)}</span></div>`).join('')}
      <div class="row total"><span>Total</span><span>${money(o.total)}</span></div>
      <div class="meta">${o.paymentMethod === 'CARD' ? 'Paid by card ending ' + esc(o.cardLast4) : 'Cash on delivery'} &middot; Ref ${esc(o.reference)}</div>
    </div>`).join('') : '<p class="meta">No orders yet.</p>';
}

document.querySelectorAll('.nav').forEach(b => b.addEventListener('click', async () => {
  try {
    if (b.dataset.view === 'orders') await loadOrders();
    else await enterShop();
  } catch (e) { msg(e.message, false); }
}));

/* ---------- sign up, log in, log out ---------- */
function authTab(login) {
  $('login-form').hidden = !login;
  $('signup-form').hidden = login;
  $('tab-login').classList.toggle('active', login);
  $('tab-signup').classList.toggle('active', !login);
}
$('tab-login').onclick = () => authTab(true);
$('tab-signup').onclick = () => authTab(false);

$('login-form').addEventListener('submit', async e => {
  e.preventDefault();
  try {
    const user = await post('/api/login', { username: $('login-user').value, password: $('login-pass').value });
    $('login-form').reset();
    await enterShop(user);
    msg('Welcome back, ' + user.username + '!', true);
  } catch (err) { msg(err.message, false); }
});

$('signup-form').addEventListener('submit', async e => {
  e.preventDefault();
  try {
    if ($('su-pass').value !== $('su-pass2').value) throw new Error('Passwords do not match');
    const user = await post('/api/signup', {
      username: $('su-user').value, email: $('su-email').value, password: $('su-pass').value
    });
    $('signup-form').reset();
    await enterShop(user);
    msg('Account created. Welcome, ' + user.username + '!', true);
  } catch (err) { msg(err.message, false); }
});

$('logout').onclick = async () => {
  try { await post('/api/logout'); } catch (e) { /* already logged out */ }
  authTab(true);
  showAuth();
  msg('Logged out.', true);
};

/* ---------- start ---------- */
(async () => {
  try { await enterShop(await api('/api/me')); } catch (e) { showAuth(); }
})();
