using BCrypt.Net;
using BookingSystem.Dto;
using BookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;

namespace BookingSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _context = dbContext;
            _configuration = configuration;
        }

        public async Task<bool> RegisterUserAsync(RegisterDto model)
        {
            try
            {
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = HashPassword(model.Password)
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                SendVerifyEmail(user.Email);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _context.Users.AsNoTracking()
                .Where(x => x.Email == model.Email)
                .SingleOrDefaultAsync();

            if (user != null)
            {
                if (!user.IsEmailVerified) return "Verify Email";

                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
                if (isPasswordCorrect)
                    return GenerateJwtToken(user);
            }
            return null;
        }

        //public async Task<UserProfileViewModel> GetUserProfileAsync(ClaimsPrincipal user)
        //{
        //    throw new NotImplementedException();
        //}

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims:claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _context.Users.AsNoTracking().AnyAsync(x => x.Email == email);
        }

        public async Task<User?> LoginUserAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.AsNoTracking().Where(x => x.Email == email && x.IsEmailVerified == true).SingleOrDefaultAsync();
                if (user != null)
                {
                    bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);
                    if (isPasswordCorrect)
                        return user;
                }
                return null;
            }
            catch (Exception ex) { return null; }

        }

        public async Task<bool> VerifyEmail(string email)
        {
            try
            {
                var user = await _context.Users.Where(x => x.Email == email).SingleOrDefaultAsync();
                if (user != null)
                {
                    user.IsEmailVerified = true;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex) { return false; }
        }

        public bool SendVerifyEmail(string parameter)
        {
            return true;
        }

        public async Task<User?> GetUserProfileAsync(int userID)
        {
            var userObj = await _context.Users.Include(x => x.UserPackages).AsNoTracking().Where(x => x.UserID == userID).SingleOrDefaultAsync();
            if(userObj == null || !userObj.IsEmailVerified) return null;
            return userObj;
        }

        public async Task<bool> CheckPasswordAsync(ChangePasswordDto model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null) throw new ApplicationException("User not found.");
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password);
            if (!isPasswordCorrect) throw new ApplicationException("Incorrect old password.");
            user.Password = HashPassword(model.NewPassword);

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _context.Users.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (user == null) throw new ApplicationException("User not found.");
            user.Password = HashPassword(model.NewPassword);

            _context.Users.Attach(user);
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;

        }
    }
}
