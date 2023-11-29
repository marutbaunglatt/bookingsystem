using BookingSystem.Dto;
using BookingSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingSystem.Services
{
    public interface IBookingService
    {
        Task<List<ClassSchedule>> GetClassSchedules();
        Task<ClassSchedule> GetClassSchedule(int id);
        Task<Booking> BookClass(BookClassDto booking);
        //Task<(Booking?, string)> BookClass(BookClassDto booking);
        Task<Booking> GetBooking(int id);
        //Task<Booking> UpdateBooking(Booking booking);
        Task<bool> DeleteBooking(int bid, int uid);

        Task<bool> AddToWaitlistAsync(BookClassDto model);

        void CheckAndRefundWaitlistUsers();
    }
}
