using Microsoft.EntityFrameworkCore;
using PetShop.Api.Domain.Entities;

namespace PetShop.Api.Database
{
    public interface IPetShopDbContext
    {
        DbSet<Employee> Employees { get; set; }
        DbSet<Client> Clients { get; set; }
        DbSet<Pet> Pets { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<Inventory> Inventory { get; set; }
        int SaveChanges();
    }
}