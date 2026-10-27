package com.shop;

import static org.junit.jupiter.api.Assertions.*;

import java.math.BigDecimal;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

class CartTest {
    private Cart cart;
    private Product mouse;

    @BeforeEach
    void setUp() {
        cart = new Cart();
        mouse = new Product(2, "Mouse", new BigDecimal("250.00"), 5);
    }

    @Test
    void addItem_updatesTotal() {
        cart.add(mouse, 2);
        assertEquals(new BigDecimal("500.00"), cart.total());
    }

    @Test
    void addSameItemTwice_mergesQuantity() {
        cart.add(mouse, 2);
        cart.add(mouse, 1);
        assertEquals(1, cart.getItems().size());
        assertEquals(3, cart.getItems().iterator().next().getQuantity());
    }

    @Test
    void addZeroQuantity_throws() {
        assertThrows(IllegalArgumentException.class, () -> cart.add(mouse, 0));
    }

    @Test
    void addMoreThanStock_throws() {
        assertThrows(IllegalArgumentException.class, () -> cart.add(mouse, 6));
    }

    @Test
    void removeMissingItem_returnsFalse() {
        assertFalse(cart.remove(99));
    }
}
