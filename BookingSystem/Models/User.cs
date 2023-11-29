namespace BookingSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }

        public ICollection<UserPackage>? UserPackages { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
