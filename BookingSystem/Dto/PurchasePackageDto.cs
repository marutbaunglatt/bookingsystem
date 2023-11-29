using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Dto
{
    public class PurchasePackageDto
    {
        [Required]
        public int UserID { get; set; }
        [Required]
        public int PackageID { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string ExpiryDate { get; set; }
        [Required]
        public string CVV { get; set; }
        [Required]
        public decimal Amount { get; set; }

    }
}
