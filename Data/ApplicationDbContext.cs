using CoachBookingApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoachBookingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Coach> Coaches { get; set; }
        public DbSet<TimeSlot> Timeslots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}