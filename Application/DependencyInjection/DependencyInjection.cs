using Application.Interfaces.Services;
using Application.Interfaces.Services.Dashboards;
using Application.Services;
using Application.Services.Dashboards;
using Application.Validators.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IShoppingCartService, ShoppingCartService>();
            services.AddScoped<ICommissionService, CommissionService>();
            services.AddScoped<ISettlementService, SettlementService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IProductCatalogService, ProductCatalogService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreActivationRequestService, StoreActivationRequestService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStoreOrderService, StoreOrderService>();
            services.AddScoped<ISellerDashboardService, SellerDashboardService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IRefundService, RefundService>();
            services.AddScoped<IReturnRequestService, ReturnRequestService>();
            services.AddScoped<IWalletService, WalletService>();

            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();

            return services;
        }
    }
}
