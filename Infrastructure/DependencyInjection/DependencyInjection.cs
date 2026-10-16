using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.Dapper;
using Application.Interfaces.Security;
using Application.Settings;
using Infrastructure.Persistence.Connection;
using Infrastructure.Persistence.Context;
using Infrastructure.Repository.Dapper;
using Infrastructure.Repository.EF;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure
            (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreLabDbContext>(options => {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            //Jwt Setting
            services.Configure<JwtSettings>( configuration.GetSection("JwtSettings"));

            var jwtSettings = configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException("JwtSettings is not configured.");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            services.AddAuthorization();


            //DataBase
            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
            
            //Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            //Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IStoreOrderRepository, StoreOrderRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IStoreActivationRepository, StoreActivationRepository>();
            services.AddScoped<IReturnRequestRepository, ReturnRequestRepository>();

            //Dapper Repository
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IStoreReadRepository, StoreReadRepository>();
            services.AddScoped<IActivationRequestReadRepository, ActivationRequestReadRepository>();
            services.AddScoped<ITransactionReadRepository, TransactionReadRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IStoreOrderReadRepository, StoreOrderReadRepository>();
            services.AddScoped<ISellerDashboardReadRepository, SellerDashboardReadRepository>();
            services.AddScoped<IAdminDashboardReadRepository, AdminDashboardReadRepository>();
            services.AddScoped<IReturnRequestReadRepository, ReturnRequestReadRepository>();



            return services;
        }
    }
}
