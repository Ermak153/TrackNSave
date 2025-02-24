using Microsoft.EntityFrameworkCore;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()");

            modelBuilder.Entity<Receipt>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Receipt>()
                .Property(r => r.UserId)
                .HasColumnType("uuid");

            modelBuilder.Entity<Receipt>()
                .HasOne(r => r.User)
                .WithMany(u => u.Receipts)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Receipt>()
                .Property(r => r.ReceiptData)
                .HasColumnType("jsonb");
        }
    }
}
