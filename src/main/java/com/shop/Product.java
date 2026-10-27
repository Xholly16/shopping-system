package com.shop;

import java.math.BigDecimal;

public class Product {
    private final int id;
    private final String name;
    private final BigDecimal price;
    private int stock;

    public Product(int id, String name, BigDecimal price, int stock) {
        if (price.signum() < 0 || stock < 0) {
            throw new IllegalArgumentException("Price and stock cannot be negative");
        }
        this.id = id;
        this.name = name;
        this.price = price;
        this.stock = stock;
    }

    public int getId() { return id; }
    public String getName() { return name; }
    public BigDecimal getPrice() { return price; }
    public int getStock() { return stock; }

    public void reduceStock(int qty) {
        if (qty <= 0 || qty > stock) {
            throw new IllegalArgumentException("Invalid quantity: " + qty);
        }
        stock -= qty;
    }

    @Override
    public String toString() {
        return String.format("[%d] %-15s R%8.2f  (stock: %d)", id, name, price, stock);
    }
}
