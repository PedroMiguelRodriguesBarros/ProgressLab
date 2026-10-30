using Sets;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class SetsDbContext : DbContext
    {
        public SetsDbContext(DbContextOptions<SetsDbContext> options) : base(options)
        {
        }

        public DbSet<Set> Sets => Set<Set>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Set>().HasKey(it => it.SetId);
            
            modelBuilder.Entity<Set>()
                .Property(it => it.SetNumber)
                .IsRequired()
                .HasColumnType("int");

            modelBuilder.Entity<Set>()
                .Property(it => it.Repetitions)
                .IsRequired()
                .HasColumnType("int");

            modelBuilder.Entity<Set>()
                .Property(it => it.Weight)
                .IsRequired()
                .HasColumnType("float");
            
            modelBuilder.Entity<Set>()
                .HasOne(it => it.Exercise)
                .WithMany()
                .HasForeignKey("ExerciseId");
        }
    }
}
