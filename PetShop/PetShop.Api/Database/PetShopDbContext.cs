using Microsoft.EntityFrameworkCore;
using PetShop.Api.Domain.Entities;

namespace PetShop.Api.Database
{   
    //Contexto de banco de dados
    public class PetShopDbContext : DbContext, IPetShopDbContext
    {
        public PetShopDbContext(DbContextOptions<PetShopDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Inventory> Inventory { get; set; }

        // Implementação explícita do método da interface IPetShopDbContext
        int IPetShopDbContext.SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
