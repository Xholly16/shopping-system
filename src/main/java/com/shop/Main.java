package com.shop;

import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Catalog catalog = new Catalog();
        Cart cart = new Cart();
        CheckoutService checkout = new CheckoutService();
        Scanner in = new Scanner(System.in);

        while (true) {
            System.out.println("\n--- ONLINE SHOP ---");
            System.out.println("1. Browse products\n2. Add to cart\n3. View cart");
            System.out.println("4. Remove from cart\n5. Checkout\n0. Exit");
            System.out.print("Choose: ");
            String choice = in.nextLine().trim();

            try {
                switch (choice) {
                    case "1" -> catalog.getAll().forEach(System.out::println);
                    case "2" -> {
                        int id = readInt(in, "Product ID: ");
                        int qty = readInt(in, "Quantity: ");
                        Product p = catalog.findById(id)
                                .orElseThrow(() -> new IllegalArgumentException("Product not found"));
                        cart.add(p, qty);
                        System.out.println("Added to cart.");
                    }
                    case "3" -> {
                        if (cart.isEmpty()) System.out.println("Cart is empty.");
                        else {
                            cart.getItems().forEach(i -> System.out.printf("%s x%d = R%.2f%n",
                                    i.getProduct().getName(), i.getQuantity(), i.getSubtotal()));
                            System.out.printf("Total: R%.2f%n", cart.total());
                        }
                    }
                    case "4" -> {
                        int id = readInt(in, "Product ID to remove: ");
                        System.out.println(cart.remove(id) ? "Removed." : "Item not in cart.");
                    }
                    case "5" -> System.out.println(checkout.checkout(cart).receipt());
                    case "0" -> { System.out.println("Goodbye!"); return; }
                    default -> System.out.println("Invalid option.");
                }
            } catch (IllegalArgumentException | IllegalStateException e) {
                System.out.println("Error: " + e.getMessage());
            }
        }
    }

    private static int readInt(Scanner in, String prompt) {
        System.out.print(prompt);
        try {
            return Integer.parseInt(in.nextLine().trim());
        } catch (NumberFormatException e) {
            throw new IllegalArgumentException("Please enter a whole number");
        }
    }
}
