using BookingSystem.Dto;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Data;

namespace BookingSystem.Services
{
    public class PackageService : IPackageService
    {
        private readonly ApplicationDbContext _context;

        public PackageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Package>> GetAvailablePackagesAsync()
        {
            return await _context.Packages.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<PackageDto>> GetPackagesByCountryAsync(string country)
        {
            // Get available packages for the specified country
            var packages = await _context.Packages.AsNoTracking().Where(x => x.Country.ToLower() == country.ToLower()).ToListAsync();

            // Map packages to PackageDto (you can use AutoMapper for this)
            var packageDtos = packages.Select(p => new PackageDto
            {
                PackageID = p.PackageID,
                Name = p.Name,
                Credits = p.Credits,
                Price = p.Price,
                ExpiryDate = p.ExpiryDate,
                Country = p.Country
            });

            return packageDtos;
        }

        public async Task<IEnumerable<PackageDto>> GetPurchasedPackagesAsync(int userID)
        {
            // Get purchased packages for the specified user
            var packages = await _context.UserPackages
                .AsNoTracking()
                .Include(x => x.Package)
                .Where(x => x.UserID == userID)
                .ToListAsync();

            // Map packages to PackageDto (you can use AutoMapper for this)
            var packageDtos = packages.Select(p => new PackageDto
            {
                PackageID = p.PackageID,
                Name = p.Package.Name,
                Credits = p.Package.Credits,
                Price = p.Package.Price,
                ExpiryDate = p.Package.ExpiryDate,
                Country = p.Package.Country
            });

            return packageDtos;
        }

        public async Task PurchasePackageAsync(PurchasePackageDto model)
        {
            // Get the user
            var user = await _context.Users.FindAsync(model.UserID);

            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            // Get the package
            var package = await _context.Packages.FindAsync(model.PackageID);

            if (package == null)
            {
                throw new ApplicationException("Package not found.");
            }

            if(package.ExpiryDate <= DateTime.UtcNow) throw new ApplicationException("Package is expired");

            if(!AddPaymentCard(model.CardNumber, model.ExpiryDate, model.CVV)) throw new ApplicationException("Invalid Card");

            if (!PaymentCharge(model.CardNumber, model.ExpiryDate, model.CVV, model.Amount)) throw new ApplicationException("Invalid Payment");



            // Purchase the package for the user
            var userPackage = new UserPackage
            {
                UserID = model.UserID,
                PackageID = model.PackageID,
                PurchaseDate = DateTime.Now,
                RemainingCredits = package.Credits,
            };
            await _context.UserPackages.AddAsync(userPackage);
            await _context.SaveChangesAsync();
        }

        public bool AddPaymentCard(string CardNumber, string ExpiryDate, string CVV)
        {
            return true;
        }



        public bool PaymentCharge(string CardNumber, string ExpiryDate, string CVV, decimal Amount)
        {
            return true;
        }
    }
}
