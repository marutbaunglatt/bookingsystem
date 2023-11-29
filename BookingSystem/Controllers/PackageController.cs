using BookingSystem.Dto;
using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePackages()
        {
            try
            {
                var packages = await _packageService.GetAvailablePackagesAsync();
                return Ok(new { status = true, message = "Success", data = packages });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }
        }

        [HttpGet("available/{country}")]
        public async Task<IActionResult> GetAvailablePackages(string country)
        {
            try
            {
                var packages = await _packageService.GetPackagesByCountryAsync(country);
                return Ok(new { status = true, message = "Success", data = packages });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { status = false, message = ex.Message });

            }
        }

        [HttpGet("purchased")]
        public async Task<IActionResult> GetPurchasedPackages(int userID)
        {
            try
            {
                // Get the user ID from the authenticated user's claims
                //var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var packages = await _packageService.GetPurchasedPackagesAsync(userID);
                return Ok(new { status = true, message = "Success", data = packages });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { status = false, message = ex.Message });

            }
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePackage([FromBody] PurchasePackageDto model)
        {
            try
            {
                // Get the user ID from the authenticated user's claims
                //var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                await _packageService.PurchasePackageAsync(model);
                return Ok(new { message = "Package purchased successfully." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { status = false, message = ex.Message });

            }
        }
    }
}
