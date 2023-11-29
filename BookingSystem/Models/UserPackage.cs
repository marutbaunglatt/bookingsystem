namespace BookingSystem.Models
{
    public class UserPackage
    {
        public int UserPackageID { get; set; }
        public int UserID { get; set; }
        public int PackageID { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int RemainingCredits { get; set; }

      
        public User User { get; set; }
        public Package Package { get; set; }
    }
}
