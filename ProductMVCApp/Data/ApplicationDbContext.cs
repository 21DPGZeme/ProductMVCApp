using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProductMVCApp.Entities;
using ProductMVCApp.Data;
using Microsoft.Extensions.Options;
using ProductMVCApp.Models;

namespace ProductMVCApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Id = "b05f7e11-7dd4-4541-a6ae-93839f075d7e", Name = "Admin", NormalizedName = "ADMIN" });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Id = "4de8f36a-ec7c-4351-8864-b69441238e2a", Name = "User", NormalizedName = "USER" });

            // Seed admin account
            var user = new IdentityUser
            {
                Id = "e02698bd-cec2-4d20-b259-c519e4855996",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
            };

            PasswordHasher<IdentityUser> ph = new PasswordHasher<IdentityUser>();
            user.PasswordHash = ph.HashPassword(user, "admin");

            modelBuilder.Entity<IdentityUser>().HasData(user);

            // Seed user role relation
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "b05f7e11-7dd4-4541-a6ae-93839f075d7e",
                    UserId = "e02698bd-cec2-4d20-b259-c519e4855996"
                }
            );

            // Seed products
            modelBuilder.Entity<Product>().HasData(
               new Product() { Id = 1, Title = "HDD 1TB", Quantity = 55, Price = (decimal)74.09 },
               new Product() { Id = 2, Title = "HDD SSD 512GB", Quantity = 102, Price = (decimal)190.99 },
               new Product() { Id = 3, Title = "RAM DDR4 16GB", Quantity = 47, Price = (decimal)80.32 }
            );

            modelBuilder.Entity<ProjectAudit>()
                .HasOne(pa => pa.User)
                .WithMany()
                .HasForeignKey(pa => pa.UserId);

            modelBuilder.Entity<ProjectAudit>()
                .HasOne(pa => pa.Product)
                .WithMany()
                .HasForeignKey(pa => pa.ProductId);
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProjectAudit> ProjectAudits { get; set; }
    }
}