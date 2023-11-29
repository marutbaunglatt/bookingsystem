using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPackage> UserPackages { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Waitlist> Waitlists { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed initial data
            SeedData(modelBuilder);

            // Other configurations...

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Users
            modelBuilder.Entity<User>().HasData(
               new User { UserID=1, Username = "user1", Email = "user1@example.com", Password = "hashed_password", IsEmailVerified = true },
                new User { UserID=2, Username = "user2", Email = "user2@example.com", Password = "hashed_password", IsEmailVerified = true }
                // Add more users as needed
            );

            // Seed Packages
            modelBuilder.Entity<Package>().HasData(
                 new Package { PackageID = 1, Name = "Basic Package SG", Credits = 5, Price = 10.99m, ExpiryDate = DateTime.UtcNow.AddMonths(2), Country = "SG" },
                new Package { PackageID = 2, Name = "Premium Package MY", Credits = 10, Price = 29.99m, ExpiryDate = DateTime.UtcNow.AddMonths(3), Country = "MY" },
                new Package { PackageID = 3, Name = "Basic Package MM", Credits = 7, Price = 15.99m, ExpiryDate = DateTime.UtcNow.AddMonths(2), Country = "MM" }
                );

            // Seed Schedules
            modelBuilder.Entity<ClassSchedule>().HasData(
                new ClassSchedule { ClassScheduleID = 1, ClassName = "Yoga Class SG", Country = "SG", StartTime = DateTime.UtcNow.AddDays(1).AddHours(9), EndTime = DateTime.UtcNow.AddDays(1).AddHours(10), RequiredCredits = 1, MaximumCapacity = 10, },
                new ClassSchedule { ClassScheduleID = 2, ClassName = "Zumba Class MY 1", Country = "MM", StartTime = DateTime.UtcNow.AddDays(1).AddHours(9), EndTime = DateTime.UtcNow.AddDays(1).AddHours(10), RequiredCredits = 2, MaximumCapacity = 2, },
                new ClassSchedule { ClassScheduleID = 3, ClassName = "Zumba Class MY 2", Country = "MY", StartTime = DateTime.UtcNow.AddDays(1).AddHours(6), EndTime = DateTime.UtcNow.AddDays(1).AddHours(7), RequiredCredits = 2, MaximumCapacity = 4, },
                new ClassSchedule { ClassScheduleID = 4, ClassName = "Zumba Class MY 3", Country = "SG", StartTime = DateTime.UtcNow.AddDays(1).AddHours(5), EndTime = DateTime.UtcNow.AddDays(1).AddHours(5), RequiredCredits = 3, MaximumCapacity = 5 },
                new ClassSchedule { ClassScheduleID = 5, ClassName = "Zumba Class MY 4", Country = "MM", StartTime = DateTime.UtcNow.AddDays(1).AddHours(4), EndTime = DateTime.UtcNow.AddDays(1).AddHours(4), RequiredCredits = 1, MaximumCapacity = 6, },
                new ClassSchedule { ClassScheduleID = 6, ClassName = "Zumba Class MY 5", Country = "MY", StartTime = DateTime.UtcNow.AddDays(1).AddHours(2), EndTime = DateTime.UtcNow.AddDays(1).AddHours(3), RequiredCredits = 2, MaximumCapacity = 7, }
                // Add more class schedules as needed
            );

            //// Seed UserPackages
            //modelBuilder.Entity<UserPackage>().HasData(

            //);

            //// Seed Bookings
            //modelBuilder.Entity<Booking>().HasData(
            //);

            // Seed Waitlists
            //modelBuilder.Entity<Waitlist>().HasData(
             
            //);
        }


    }


}
