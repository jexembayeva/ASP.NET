using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.Customers;
using PromoCodeFactory.Core.Domain.PromoCode;

namespace PromoCodeFactory.DataAccess.Data;

public class PromoCodeFactoryDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DbSet<PromoCode> PromoCodes { get; set; }

    public DbSet<CustomerPreference> CustomerPreferences { get; set; }

    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureEmployees(builder);
        ConfigureRoles(builder);
        ConfigureCustomers(builder);
        ConfigurePromoCodes(builder);
        ConfigureCustomerPreferences(builder);
    }

    private void ConfigureEmployees(ModelBuilder builder)
    {
        builder.Entity<Employee>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .HasMaxLength(100);

            entity.HasMany(e => e.Roles)
                .WithMany();
        });
    }

    private void ConfigureRoles(ModelBuilder builder)
    {
        builder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .HasMaxLength(50);

            entity.Property(x => x.Description)
                .HasMaxLength(200);
        });
    }

    private void ConfigureCustomers(ModelBuilder builder)
    {
        builder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .HasMaxLength(100);
        });
    }

    private void ConfigurePromoCodes(ModelBuilder builder)
    {
        builder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Code)
                .HasMaxLength(30);

            entity.Property(x => x.ServiceInfo)
                .HasMaxLength(100);

            entity.HasOne(x => x.Preference)
                .WithMany()
                .HasForeignKey(x => x.PreferenceId);

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.PromoCodes)
                .HasForeignKey(x => x.CustomerId);
        });
    }

    private void ConfigureCustomerPreferences(ModelBuilder builder)
    {
        builder.Entity<CustomerPreference>(entity =>
        {
            entity.HasKey(x => new { x.CustomerId, x.PreferenceId });

            entity.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerPreferences)
                .HasForeignKey(x => x.CustomerId);

            entity.HasOne(x => x.Preference)
                .WithMany(x => x.CustomerPreferences)
                .HasForeignKey(x => x.PreferenceId);
        });
    }
}