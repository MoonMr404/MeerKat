using Microsoft.EntityFrameworkCore;
using ServerBackend.Models;

namespace ServerBackend.Data;

public class MeerkatContext : DbContext
{
    public MeerkatContext(DbContextOptions<MeerkatContext> options) : base(options) { } //Costruttore per Dependecy injection
    
    /*
     * Ogni DbSet è una tabella all'interno dell'ORM
     */
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            //Impedisce l'eliminazione di un utente con almeno un team a capo
            entity.HasMany(u => u.ManagedTeams)
                .WithOne(t => t.Manager)
                .OnDelete(DeleteBehavior.Restrict); 
        });

        modelBuilder.Entity<Team>(entity =>
        {
            // Relazione con Manager (User)
            entity.HasOne(t => t.Manager)
                .WithMany(u => u.ManagedTeams)
                .HasForeignKey("ManagerId");

            // Relazione con Membri (User)
            entity.HasMany(t => t.Members)
                .WithMany(u => u.MemberOfTeams)
                .UsingEntity(j => j.ToTable("TeamMembers")); // Crea la tabella di giunzione
        });
    }
}