using Breakfast.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Breakfast.Api.Data
{
    public class BreakfastContext : DbContext
    {
        public BreakfastContext(DbContextOptions<BreakfastContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<BreakfastItem>()
                .Property(x => x.Id)
                .HasValueGenerator<SequentialGuidValueGenerator>();
        }

        public DbSet<BreakfastItem> BreakfastItems { get; set; }
    }
}
