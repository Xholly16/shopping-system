package com.shop;

import static org.junit.jupiter.api.Assertions.*;

import java.math.BigDecimal;
import org.junit.jupiter.api.Test;

class CheckoutServiceTest {
    @Test
    void checkout_reducesStock_andClearsCart() {
        Product p = new Product(1, "Laptop", new BigDecimal("1000.00"), 3);
        Cart cart = new Cart();
        cart.add(p, 2);

        Order order = new CheckoutService().checkout(cart);

        assertEquals(new BigDecimal("2000.00"), order.getTotal());
        assertEquals(1, p.getStock());
        assertTrue(cart.isEmpty());
    }

    @Test
    void checkout_emptyCart_throws() {
        assertThrows(IllegalStateException.class,
                () -> new CheckoutService().checkout(new Cart()));
    }
}
