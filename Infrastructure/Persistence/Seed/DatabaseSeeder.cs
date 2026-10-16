using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(StoreLabDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
            return;

        // =========================    Users   =========================

        var admin = new User(
            "Admin",
            "User",
            "admin@storelab.com",
            "Admin123!",
            "09120000001",
            UserRole.Admin);

        var seller = new User(
            "Seller",
            "User",
            "seller@storelab.com",
            "Seller123!",
            "09120000002",
            UserRole.Seller);

        var buyer = new User(
            "Buyer",
            "User",
            "buyer@storelab.com",
            "Buyer123!",
            "09120000003",
            UserRole.Buyer);

        context.Users.AddRange(admin, seller, buyer);

        await context.SaveChangesAsync();

        // =========================    Wallets =========================



        var buyerWallet = new Wallet(
            buyer.Id,
            WalletType.Buyer);

        buyerWallet.Deposit(1_000_000);

        var sellerWallet = new Wallet(
            seller.Id,
            WalletType.Seller);

        var platformWallet = new Wallet(
            null,
            WalletType.Platform);

        var escrowWallet = new Wallet(
            null,
            WalletType.Escrow);

        context.Wallets.AddRange(
            buyerWallet,
            sellerWallet,
            platformWallet,
            escrowWallet);

        await context.SaveChangesAsync();

        // =========================    Store   =========================



        var store = new Store(
            seller.Id,
            "StoreLab Store",
            "StoreLab demo store");

        context.Stores.Add(store);

        await context.SaveChangesAsync();

        // =========================  Products  =========================



        var products = new List<Product>
        {
            new Product(
                store.Id,
                "Laptop",
                "Professional laptop",
                45_000_000,
                10),

            new Product(
                store.Id,
                "Mechanical Keyboard",
                "Mechanical gaming keyboard",
                3_500_000,
                20),

            new Product(
                store.Id,
                "Gaming Mouse",
                "High precision gaming mouse",
                2_000_000,
                25),

            new Product(
                store.Id,
                "Headset",
                "Gaming headset",
                4_500_000,
                15),

            new Product(
                store.Id,
                "Monitor",
                "27 inch monitor",
                12_000_000,
                8)
        };

        context.Products.AddRange(products);

        await context.SaveChangesAsync();

        // =========================    Shopping Cart   =========================



        var shoppingCart = new ShoppingCart(buyer.Id);

        context.ShoppingCarts.Add(shoppingCart);

        await context.SaveChangesAsync();
    }
}