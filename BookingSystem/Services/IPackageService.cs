using BookingSystem.Dto;
using BookingSystem.Models;

namespace BookingSystem.Services
{
    public interface IPackageService
    {
        Task<IEnumerable<Package>> GetAvailablePackagesAsync();
        Task<IEnumerable<PackageDto>> GetPackagesByCountryAsync(string country);
        Task<IEnumerable<PackageDto>> GetPurchasedPackagesAsync(int userID);
        Task PurchasePackageAsync(PurchasePackageDto model);
    }
}
