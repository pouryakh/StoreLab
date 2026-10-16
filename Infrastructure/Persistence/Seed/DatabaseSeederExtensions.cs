using Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence.Seed;

public static class DatabaseSeederExtensions
{
    public static async Task SeedDatabaseAsync(
        this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<StoreLabDbContext>();

        await DatabaseSeeder.SeedAsync(context);
    }
}