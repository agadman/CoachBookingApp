using System.ComponentModel.DataAnnotations;

namespace CoachBookingApp.Models;

public class TimeSlot
{
    public int Id { get; set; }
    [Display(Name = "Starttid")]
    public DateTime StartTime { get; set; } // Detta sparas i UTC - använder DateTimeExtensions för att konvertera till svensk tid i vyerna
    [Display(Name = "Sluttid")]
    public DateTime EndTime { get; set; }   // Detta sparas i UTC - använder DateTimeExtensions för att konvertera till svensk tid i vyerna
    [Display(Name = "Coach")]
    public int CoachId { get; set; }
    [Display(Name = "Coach")]
    public Coach? Coach { get; set; }
    public Booking? Booking { get; set; }
}