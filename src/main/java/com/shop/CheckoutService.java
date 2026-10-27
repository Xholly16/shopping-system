package com.shop;

import java.util.ArrayList;

public class CheckoutService {
    private int nextOrderId = 1;

    public Order checkout(Cart cart) {
        if (cart.isEmpty()) {
            throw new IllegalStateException("Cart is empty");
        }
        for (CartItem item : cart.getItems()) {
            item.getProduct().reduceStock(item.getQuantity());
        }
        Order order = new Order(nextOrderId++, new ArrayList<>(cart.getItems()), cart.total());
        cart.clear();
        return order;
    }
}
