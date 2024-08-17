using Microsoft.EntityFrameworkCore;

namespace NimblePros.SampleWeb;

public class CustomerContext(DbContextOptions<CustomerContext> options) : DbContext(options)
{
  public DbSet<Customer> Customers { get; set; }
}
