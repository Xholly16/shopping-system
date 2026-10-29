package com.shop;

import java.util.ArrayList;
import java.util.List;

public class CheckoutService {
    private int nextOrderId = 1;
    private final List<Order> orders = new ArrayList<>();
    private final PaymentService payments = new PaymentService();

    public synchronized Order checkout(Cart cart) {
        return finish(cart, null, null);
    }

    public synchronized Order checkout(Cart cart, String username, PaymentService.Method method,
                                       String cardNumber, String expiry, String cvv) {
        verifyCart(cart);
        PaymentService.PaymentResult payment =
                payments.pay(method, cardNumber, expiry, cvv, cart.total());
        return finish(cart, username, payment);
    }

    public synchronized List<Order> ordersFor(String username) {
        List<Order> result = new ArrayList<>();
        for (Order o : orders) {
            if (username.equals(o.getUsername())) result.add(o);
        }
        return result;
    }

    private void verifyCart(Cart cart) {
        if (cart.isEmpty()) {
            throw new IllegalStateException("Cart is empty");
        }
        for (CartItem item : cart.getItems()) {
            if (item.getQuantity() > item.getProduct().getStock()) {
                throw new IllegalArgumentException("Not enough stock for " + item.getProduct().getName());
            }
        }
    }

    private Order finish(Cart cart, String username, PaymentService.PaymentResult payment) {
        verifyCart(cart);
        for (CartItem item : cart.getItems()) {
            item.getProduct().reduceStock(item.getQuantity());
        }
        Order order = new Order(nextOrderId++, new ArrayList<>(cart.getItems()), cart.total(),
                username,
                payment == null ? null : payment.method().name(),
                payment == null ? null : payment.cardLast4(),
                payment == null ? null : payment.reference());
        orders.add(order);
        cart.clear();
        return order;
    }
}
