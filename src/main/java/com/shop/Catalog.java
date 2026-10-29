package com.shop;

import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

public class Catalog {
    private final List<Product> products = new ArrayList<>();

    public Catalog() {
        products.add(new Product(1, "Laptop", new BigDecimal("12999.99"), 5));
        products.add(new Product(2, "Mouse", new BigDecimal("249.50"), 20));
        products.add(new Product(3, "Keyboard", new BigDecimal("599.00"), 10));
        products.add(new Product(4, "Headphones", new BigDecimal("899.99"), 8));
        products.add(new Product(5, "USB Cable", new BigDecimal("79.00"), 50));
        products.add(new Product(6, "Monitor", new BigDecimal("2499.00"), 6));
        products.add(new Product(7, "Webcam", new BigDecimal("699.00"), 12));
        products.add(new Product(8, "Backpack", new BigDecimal("449.00"), 15));
        products.add(new Product(9, "Power Bank", new BigDecimal("399.00"), 25));
        products.add(new Product(10, "Phone Stand", new BigDecimal("129.00"), 40));
    }

    public List<Product> getAll() { return products; }

    public Optional<Product> findById(int id) {
        return products.stream().filter(p -> p.getId() == id).findFirst();
    }
}
