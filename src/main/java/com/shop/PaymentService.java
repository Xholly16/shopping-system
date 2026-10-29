package com.shop;

import java.math.BigDecimal;
import java.time.YearMonth;
import java.util.UUID;

public class PaymentService {
    public enum Method { CARD, CASH_ON_DELIVERY }

    public record PaymentResult(Method method, String cardLast4, String reference) {}

    public PaymentResult pay(Method method, String cardNumber, String expiry,
                             String cvv, BigDecimal amount) {
        if (method == null) {
            throw new IllegalArgumentException("Choose a payment method");
        }
        if (amount == null || amount.signum() <= 0) {
            throw new IllegalArgumentException("Nothing to pay");
        }
        if (method == Method.CASH_ON_DELIVERY) {
            return new PaymentResult(method, null, reference());
        }
        String digits = cardNumber == null ? "" : cardNumber.replaceAll("[\\s-]", "");
        if (!digits.matches("\\d{13,19}") || !luhn(digits)) {
            throw new IllegalArgumentException("Invalid card number");
        }
        checkExpiry(expiry);
        if (cvv == null || !cvv.matches("\\d{3,4}")) {
            throw new IllegalArgumentException("Invalid CVV");
        }
        return new PaymentResult(method, digits.substring(digits.length() - 4), reference());
    }

    public static boolean luhn(String digits) {
        int sum = 0;
        boolean dbl = false;
        for (int i = digits.length() - 1; i >= 0; i--) {
            int d = digits.charAt(i) - '0';
            if (dbl) {
                d *= 2;
                if (d > 9) d -= 9;
            }
            sum += d;
            dbl = !dbl;
        }
        return sum % 10 == 0;
    }

    private static void checkExpiry(String expiry) {
        if (expiry == null || !expiry.matches("(0[1-9]|1[0-2])/\\d{2}")) {
            throw new IllegalArgumentException("Expiry must be in MM/YY format");
        }
        int month = Integer.parseInt(expiry.substring(0, 2));
        int year = 2000 + Integer.parseInt(expiry.substring(3));
        if (YearMonth.of(year, month).isBefore(YearMonth.now())) {
            throw new IllegalArgumentException("Card has expired");
        }
    }

    private static String reference() {
        return "PAY-" + UUID.randomUUID().toString().substring(0, 8).toUpperCase();
    }
}
