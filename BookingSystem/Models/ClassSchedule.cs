namespace BookingSystem.Models
{
    public class ClassSchedule
    {
        public int ClassScheduleID { get; set; }
        public string ClassName { get; set; }
        public string Country { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RequiredCredits { get; set; }
        public int MaximumCapacity { get; set; }
        public bool? RefundDone { get; set; }
        public int? PackageID { get; set; }
        public Package? Package { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Waitlist>? Waitlists { get; set; }
    }
}
