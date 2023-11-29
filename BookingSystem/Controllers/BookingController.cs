using BookingSystem.Dto;
using BookingSystem.Models;
using BookingSystem.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("class-schedules")]
        public async Task<ActionResult<List<ClassSchedule>>> GetClassSchedules()
        {
            var schedules = await _bookingService.GetClassSchedules();
            return Ok(new { status = true, message = "Success", data = schedules });
        }

        [HttpGet("class-schedule/{id}")]
        public async Task<ActionResult<ClassSchedule>> GetClassSchedule(int id)
        {
            var schedule = await _bookingService.GetClassSchedule(id);

            if (schedule == null)
            {
                return NotFound(new { status = false, message = "Not Found" });
            }
            return Ok(new { status = true, message = "Success", data = schedule });
        }

        [HttpPost("book-class")]
        public async Task<ActionResult<Booking>> BookClass(BookClassDto model)
        {
            try
            {
                var book = await _bookingService.BookClass(model);
                return Ok(new {status = true, message = "Class booked successfully.", data = book });
            }
            catch (Exception ex)
            {
                return BadRequest(new {status = false, message = ex.Message });
            }
        }

        [HttpGet("booking/{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _bookingService.GetBooking(id);
            if (booking == null)
            {
                return NotFound(new { status = false, message = "Not Found" });
            }
            return Ok(new { status = true, message = "Success", data = booking });
        }

        //[HttpPut("update-booking")]
        //public async Task<ActionResult<Booking>> UpdateBooking(Booking booking)
        //{
        //    var existingBooking = await _bookingService.UpdateBooking(booking);

        //    if (existingBooking == null)
        //    {
        //        return NotFound(new { status = false, message = "Not Found" });
        //    }
        //    return Ok(new { status = true, message = "Success", data = existingBooking });
        //}

        [HttpDelete("cancel-booking/{bid}/{uid}")]
        public async Task<ActionResult<Booking>> DeleteBooking(int bid, int uid)
        {
            try
            {
                var booking = await _bookingService.DeleteBooking(bid, uid);

                if (booking == true)
                {
                    return Ok(new { status = false, message = "Booking canceled successfully" });
                }
                else if (booking == false)
                    return Forbid("You are not allowed to cancel this booking");
                return Ok(new { status = true, message = "Success", data = booking });

            }catch (Exception ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }

        }

        [HttpPost("waitlist/add")]
        public async Task<IActionResult> AddToWaitlist([FromBody] BookClassDto model)
        {
            try
            {
                var result = await _bookingService.AddToWaitlistAsync(model);
                RecurringJob.AddOrUpdate(() => _bookingService.CheckAndRefundWaitlistUsers(), Cron.Minutely);
                return Ok(new { status = true, message = "Added to waitlist successfully." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }
        }
    }
}
