namespace BookingSystem.Models
{
    public class Package
    {
        public int PackageID { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Country { get; set; }

        public ICollection<UserPackage>? UserPackages { get; set; }
        public ICollection<ClassSchedule> ClassSchedules { get; set; }
    }
}
