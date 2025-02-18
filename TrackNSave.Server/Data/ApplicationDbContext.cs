using Microsoft.EntityFrameworkCore;
using TrackNSave.Server.Models;

namespace TrackNSave.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()");
        }
    }
}
