package com.shop;

import java.math.BigDecimal;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.Map;

public class Cart {
    private final Map<Integer, CartItem> items = new LinkedHashMap<>();

    public void add(Product product, int qty) {
        if (qty <= 0) {
            throw new IllegalArgumentException("Quantity must be positive");
        }
        CartItem existing = items.get(product.getId());
        int newQty = (existing == null ? 0 : existing.getQuantity()) + qty;
        if (newQty > product.getStock()) {
            throw new IllegalArgumentException("Not enough stock for " + product.getName());
        }
        items.put(product.getId(), new CartItem(product, newQty));
    }

    public boolean remove(int productId) {
        return items.remove(productId) != null;
    }

    public BigDecimal total() {
        return items.values().stream()
                .map(CartItem::getSubtotal)
                .reduce(BigDecimal.ZERO, BigDecimal::add);
    }

    public Collection<CartItem> getItems() { return items.values(); }
    public boolean isEmpty() { return items.isEmpty(); }
    public void clear() { items.clear(); }
}
