using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz_UI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Static seed for a development user. All values are hardcoded to avoid
                // dynamic values in HasData (PasswordHash, SecurityStamp, ConcurrencyStamp, etc.).
                // This prevents the "PendingModelChangesWarning" caused by using Guid.NewGuid()
                // or computing a password hash at runtime inside OnModelCreating.
                var userId = "001";
                var user = new ApplicationUser
                {
                    Id = userId,
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM",
                    EmailConfirmed = true,
                    // Precomputed password hash (for "Password1234!") copied from an existing migration
                    PasswordHash = "AQAAAAIAAYagAAAAEFHM75ycAE7U4myexV9bj22BEDaScpKLVvmpjpdNLeNeF5q79wrMPSJ3jlYx5kemGg==",
                    SecurityStamp = "9cc2fdc4-66cb-410a-8694-6b3713c72c8e",
                    ConcurrencyStamp = "7787a1c3-96c0-4c41-af34-6f27ab092c8a"
                };

                modelBuilder.Entity<ApplicationUser>().HasData(user);
            }
        }

    // Seeda data med OnModelCreate (gjort)
    // Migration igen och update
}
