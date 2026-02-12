using CoachBookingApp.Models;

public class TimeSlot
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int CoachId { get; set; }
    public Coach? Coach { get; set; }
}
