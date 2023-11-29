namespace BookingSystem.Models
{
    public class Waitlist
    {
        public int WaitlistId { get; set; }
        public int UserId { get; set; }
        public int ClassScheduleId { get; set; }
        public DateTime WaitlistTime { get; set; }
        // Other waitlist-related properties

        public User User { get; set; }
        public ClassSchedule ClassSchedule { get; set; }
    }
}
