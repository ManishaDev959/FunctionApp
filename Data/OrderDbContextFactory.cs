using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderProcessing.Functions.Data;

public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=MANISHA\\SQLEXPRESS;Database=orderdb123;Trusted_Connection=True;TrustServerCertificate=True;");

        return new OrderDbContext(optionsBuilder.Options);
    }
}