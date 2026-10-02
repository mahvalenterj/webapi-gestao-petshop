using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetShop.Api.Database;

namespace PetShop.Api.Tests.Infrastructure
{
    public class PetShopApiFactory : WebApplicationFactory<Program>
    {
        private readonly string databaseName = $"PetShopTests-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("DatabaseServerName", "localhost");
            builder.UseSetting("DatabasePassword", "not-used");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<PetShopDbContext>));
                services.Remove(descriptor);

                services.AddDbContext<IPetShopDbContext, PetShopDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName));
            });
        }

        public async Task SeedAsync(Func<PetShopDbContext, Task> seed)
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PetShopDbContext>();
            await seed(context);
            await context.SaveChangesAsync();
        }
    }
}
