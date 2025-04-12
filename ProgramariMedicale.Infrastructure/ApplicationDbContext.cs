using MedProgramari.Domain.Data;
using MedProgramari.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ProgramariMedicale.Domain.Entity;

namespace ProgramariMedicale.Infrastructure;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Pacient> Pacienti { get; set; }
    public DbSet<MedIndtitut> InstitutiiPublice { get; set; }
    public DbSet<Departamente>  Departamente { get; set; }
    public DbSet<ProgramariPaciet> Programari { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> PacientRoles { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var Role = new List<Role>()
        {
            new Role(){Id = Guid.NewGuid(),Name = "Pacient"},
            new Role(){Id = Guid.NewGuid(),Name = "Oaspete"},
        };
        modelBuilder.Entity<Role>().HasData(Role);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.IdUser, ur.IdRole });  // Setează cheia primară compusă

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Pacient)    // UserRole are un User
            .WithMany(u => u.UserRoles) // User are multe UserRoles
            .HasForeignKey(ur => ur.IdUser); // Cheia străină

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)    // UserRole are un Role
            .WithMany(r => r.UserRoles) // Role are multe UserRoles
            .HasForeignKey(ur => ur.IdRole); // Cheia străină

        modelBuilder.Entity<ProgramariPaciet>()
            .HasKey(pr=> new { pr.Idnp_Pacient, pr.Idnp_Med }); // Setează cheia primară compusă


    }

}