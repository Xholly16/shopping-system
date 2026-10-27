package com.shop;

import java.math.BigDecimal;
import java.util.List;

public class Order {
    private final int id;
    private final List<CartItem> items;
    private final BigDecimal total;

    public Order(int id, List<CartItem> items, BigDecimal total) {
        this.id = id;
        this.items = items;
        this.total = total;
    }

    public int getId() { return id; }
    public BigDecimal getTotal() { return total; }
    public List<CartItem> getItems() { return items; }

    public String receipt() {
        StringBuilder sb = new StringBuilder("\n===== RECEIPT (Order #" + id + ") =====\n");
        for (CartItem i : items) {
            sb.append(String.format("%-15s x%d  R%.2f%n",
                    i.getProduct().getName(), i.getQuantity(), i.getSubtotal()));
        }
        sb.append(String.format("TOTAL: R%.2f%n", total));
        return sb.toString();
    }
}
