namespace CoachBookingApp.Models;

public class TimeSlot
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; } // Detta sparas i UTC
    public DateTime EndTime { get; set; }   // Detta sparas i UTC
    public int CoachId { get; set; }
    public Coach? Coach { get; set; }
    public Booking? Booking { get; set; }
}