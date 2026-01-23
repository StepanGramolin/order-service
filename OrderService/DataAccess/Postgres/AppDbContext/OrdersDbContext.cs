using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.Models;

namespace OrderService.DataAccess.Postgres.AppDbContext;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(b =>
        {
            b.ToTable("orders");

            // PK теперь OrderId
            b.HasKey(x => x.OrderId);

            b.Property(x => x.ProductId).IsRequired();
            b.Property(x => x.Amount).IsRequired();
            b.Property(x => x.EmailClient).IsRequired();
            b.Property(x => x.Price).HasColumnType("numeric(18,2)").IsRequired();
            b.Property(x => x.PhoneNumber).IsRequired();
        });
    }
}