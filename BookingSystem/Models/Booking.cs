namespace BookingSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int ClassScheduleID { get; set; }
        public DateTime BookingTime { get; set; }
        //public bool IsWaitlisted { get; set; }


        public User User { get; set; }
        public ClassSchedule ClassSchedule { get; set; }
    }
}
