package com.shop;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import java.util.List;

public class Order {
    private final int id;
    private final List<CartItem> items;
    private final BigDecimal total;
    private final String username;
    private final String paymentMethod;
    private final String cardLast4;
    private final String reference;
    private final String createdAt;

    public Order(int id, List<CartItem> items, BigDecimal total) {
        this(id, items, total, null, null, null, null);
    }

    public Order(int id, List<CartItem> items, BigDecimal total, String username,
                 String paymentMethod, String cardLast4, String reference) {
        this.id = id;
        this.items = items;
        this.total = total;
        this.username = username;
        this.paymentMethod = paymentMethod;
        this.cardLast4 = cardLast4;
        this.reference = reference;
        this.createdAt = LocalDateTime.now().withNano(0).toString();
    }

    public int getId() { return id; }
    public BigDecimal getTotal() { return total; }
    public List<CartItem> getItems() { return items; }
    public String getUsername() { return username; }
    public String getPaymentMethod() { return paymentMethod; }
    public String getCardLast4() { return cardLast4; }
    public String getReference() { return reference; }
    public String getCreatedAt() { return createdAt; }

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
