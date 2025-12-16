using Microsoft.EntityFrameworkCore;
using CLIProfessor.Domain.Entities;

namespace CLIProfessor.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<LearnedCorrection> LearnedCorrections { get; set; }
    public DbSet<CommandRequest> CommandRequests { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<LearnedCorrection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Embedding).HasColumnType("vector(768)");
        });

        modelBuilder.Entity<CommandRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsOne(e => e.Context);
        });
    }
}
