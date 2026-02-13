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
        public string Status { get; set; } = "Booked"; // Booked, Cancelled, Completed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // UtcNow to get local time in UI

        [Required]
        public int TimeSlotId { get; set; }
        public TimeSlot? TimeSlot { get; set; }
        public string? UserId { get; set; } = null!; // from Identity
    }
}