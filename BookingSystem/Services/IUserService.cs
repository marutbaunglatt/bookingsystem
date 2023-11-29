using BookingSystem.Dto;
using BookingSystem.Models;
using System.Security.Claims;

namespace BookingSystem.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(RegisterDto model);
        Task<string> LoginAsync(LoginDto model);
        //Task<UserProfileViewModel> GetUserProfileAsync(ClaimsPrincipal user);
        Task<bool> IsEmailTakenAsync(string email);

        Task<User?> LoginUserAsync(string email, string password);

        Task<bool> VerifyEmail(string email);

        Task<User?> GetUserProfileAsync(int userID);
    }
}
