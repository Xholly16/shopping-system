package com.shop;

import static org.junit.jupiter.api.Assertions.*;

import java.math.BigDecimal;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

class PaymentServiceTest {
    private static final BigDecimal AMOUNT = new BigDecimal("100.00");
    private final PaymentService pay = new PaymentService();

    @Test
    void validCard_keepsOnlyLast4() {
        PaymentService.PaymentResult r =
                pay.pay(PaymentService.Method.CARD, "4242 4242 4242 4242", "12/40", "123", AMOUNT);
        assertEquals("4242", r.cardLast4());
        assertTrue(r.reference().startsWith("PAY-"));
    }

    @Test
    void cashOnDelivery_needsNoCardDetails() {
        PaymentService.PaymentResult r =
                pay.pay(PaymentService.Method.CASH_ON_DELIVERY, null, null, null, AMOUNT);
        assertNull(r.cardLast4());
    }

    @Test
    void cardFailingLuhn_isRejected() {
        assertThrows(IllegalArgumentException.class,
                () -> pay.pay(PaymentService.Method.CARD, "4242424242424241", "12/40", "123", AMOUNT));
    }

    @Test
    void expiredCard_isRejected() {
        assertThrows(IllegalArgumentException.class,
                () -> pay.pay(PaymentService.Method.CARD, "4242424242424242", "01/20", "123", AMOUNT));
    }

    @ParameterizedTest
    @ValueSource(strings = {"", "12", "12345", "abc"})
    void badCvv_isRejected(String cvv) {
        assertThrows(IllegalArgumentException.class,
                () -> pay.pay(PaymentService.Method.CARD, "4242424242424242", "12/40", cvv, AMOUNT));
    }

    @Test
    void zeroAmount_isRejected() {
        assertThrows(IllegalArgumentException.class,
                () -> pay.pay(PaymentService.Method.CARD, "4242424242424242", "12/40", "123", BigDecimal.ZERO));
    }

    @Test
    void luhn_knownValues() {
        assertTrue(PaymentService.luhn("4242424242424242"));
        assertFalse(PaymentService.luhn("4242424242424241"));
    }
}
