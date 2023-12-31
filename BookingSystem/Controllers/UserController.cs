﻿using BookingSystem.Dto;
using BookingSystem.Models;
using BookingSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        public UserController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { status = false, message = ModelState });
            }

            // Check if the email already exists
            var isEmailTaken = await _userService.IsEmailTakenAsync(model.Email);
            if (isEmailTaken)
            {
                return BadRequest(new { status = false, messag = "Email is already taken" });
            }

            // Continue with the registration logic
            await _userService.RegisterUserAsync(model);

            return Ok(new { status = true, message = "User registered successfully. Please check your email for verification." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _userService.LoginAsync(model);

            if (token == null)
                return Unauthorized("Invalid credentials");

            if(token == "Verify Email")
                return Unauthorized("Verify Your Email");

            return Ok(new { status = true, messag="success", data = token });
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            bool isOk = await _userService.VerifyEmail(email);
            if(isOk) return Ok(new {status=true, message = "Success"});

            return BadRequest(new { status = false, message = "Fail" });
        }
         
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize]
        [HttpGet("profile/{email}")]
        public async Task<IActionResult> Profile(string email)
        {
            var user = await _userService.GetUserProfileAsync(email);
            if (user == null)
                return NotFound();
            return Ok(new { status = true, message = "success", data = user });
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { status = false, message = ModelState });
                }

                // Verify the current password
                var res = await _userService.CheckPasswordAsync(model);

                return Ok(new { status = false, message = "Password changed successfully" });
            }catch (Exception ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }
           
        }

        [Authorize]
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { status = false, message = ModelState });
                }

                // Verify the current password
                var res = await _userService.ResetPasswordAsync(model);

                return Ok(new { status = false, message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }
        }
    }
}
