namespace BookingSystem.Dto
{
    public class PackageDto
    {
        public int PackageID { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Country { get; set; }
    }
}
