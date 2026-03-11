using System.ComponentModel.DataAnnotations;

namespace CoachBookingApp.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Namn")]
        public string CustomerName { get; set; } = null!;

        [Required, EmailAddress]
        [Display(Name = "E-post")]
        public string CustomerEmail { get; set; } = null!;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Booked"; // Status ska vara Booked, Cancelled, Completed, No show

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        [Required]
        [Display(Name = "Tid")]
        public int TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }
        public string? UserId { get; set; } = null!; // Denna FK från Identity
    }
}