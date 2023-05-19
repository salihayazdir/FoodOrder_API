using FoodOrder_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder_API.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().HasData(
            new Item()
            {
                Id = 1,
                Name = "Item1",
                Description = "Descr..",
                Price = 10,
            },
            new Item()
            {
                Id = 2,
                Name = "Item2",
                Description = "Descr..",
                Price = 20,
            },
            new Item()
            {
                Id = 3,
                Name = "Item3",
                Description = "Descr..",
                Price = 30,
            }
            );
        }
    }
}
