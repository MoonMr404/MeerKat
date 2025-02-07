using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace ServerBackend.Data;

public class MeerkatDatabase : DbContext
{
    public MeerkatDatabase(DbContextOptions<MeerkatDatabase> options) : base(options) { } //Costruttore per Dependecy injection
    
    /*
     * Ogni DbSet è una tabella all'interno dell'ORM
     */
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Surname).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.BirthDate).IsRequired();
            entity.Property(u => u.Image);
        });
    }
}