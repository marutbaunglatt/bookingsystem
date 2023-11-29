using BookingSystem.Dto;
using BookingSystem.Models;
using BookingSystem1.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace BookingSystem.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRedisCache _cache;
        public BookingService(ApplicationDbContext context, IRedisCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> AddToWaitlistAsync(BookClassDto model)
        {
            var waitlist = await _context.Waitlists
                .AsNoTracking()
                .Where(x => x.ClassScheduleId == model.ClassScheduleID && x.UserId == model.UserID)
                .FirstOrDefaultAsync();


            if (waitlist != null) throw new ApplicationException("User is already in the waitlist");
            var schedule = await _context.ClassSchedules.FindAsync(model.ClassScheduleID);
            if (schedule == null) throw new ApplicationException("Class schedule not found");

            var userPackage = await _context.UserPackages
              .Include(x => x.Package)
              .Where(x => x.UserID == model.UserID && x.Package.Country == schedule.Country && x.RemainingCredits > 0)
              .FirstOrDefaultAsync();
            if (userPackage == null)
                throw new ApplicationException("User does not have a package for this country.");

            if (userPackage.RemainingCredits < schedule.RequiredCredits)
                throw new ApplicationException("Insufficient credits in the user's package.");

            var waitlistEntry = new Waitlist
            {
                UserId = model.UserID,
                ClassScheduleId = model.ClassScheduleID,
                WaitlistTime = DateTime.Now
            };

            userPackage.RemainingCredits -= schedule.RequiredCredits;
            _context.UserPackages.Attach(userPackage);
            _context.Entry(userPackage).State = EntityState.Modified;

            await _context.Waitlists.AddAsync(waitlistEntry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Booking> BookClass(BookClassDto model)
        {
            var cachedata = await _cache.GetData<ClassScheduleDto>($"classobj{model.ClassScheduleID}");
            if (cachedata != null && cachedata.MaximumCapacity <= 0)
                throw new ApplicationException($"The class is full.You can join the waitlist for {cachedata.ClassName}");

            var user = await _context.Users.FindAsync(model.UserID);
            if (user == null)
                throw new ApplicationException("User not found");

            var schedule = await _context.ClassSchedules.FindAsync(model.ClassScheduleID);
            if (schedule == null)
                throw new ApplicationException("Class schedule not found");

            //if (schedule.MaximumCapacity <= 0)
            //    return (null, "Class is full");

            var userPackage = await _context.UserPackages
                //.AsNoTracking()
                .Include(x => x.Package)
                .Where(x => x.UserID == user.UserID && x.Package.Country == schedule.Country && x.RemainingCredits > 0)
                .FirstOrDefaultAsync();
            if (userPackage == null)
                throw new ApplicationException("User does not have a package for this country.");

            if (userPackage.RemainingCredits < schedule.RequiredCredits)
                throw new ApplicationException("Insufficient credits in the user's package.");

            var existingBooking = await _context.Bookings.
                FirstOrDefaultAsync(b => b.ClassScheduleID == model.ClassScheduleID && b.UserID == model.UserID);

            if (existingBooking != null)
                throw new ApplicationException("User already booked for this class");

            var waitlist = await _context.Waitlists
                .FirstOrDefaultAsync(w => w.ClassScheduleId == model.ClassScheduleID);

            if (waitlist != null)
                throw new ApplicationException("User already on waitlist for this class");

            var booking = new Booking
            {
                UserID = user.UserID,
                ClassScheduleID = model.ClassScheduleID,
                BookingTime = DateTime.UtcNow,
            };


            userPackage.RemainingCredits -= schedule.RequiredCredits;
            _context.UserPackages.Attach(userPackage);
            _context.Entry(userPackage).State = EntityState.Modified;


            schedule.MaximumCapacity = schedule.MaximumCapacity - 1;
            _context.ClassSchedules.Attach(schedule);
            _context.Entry(schedule).State = EntityState.Modified;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var classSj = new ClassScheduleDto
            {
                ClassScheduleID = schedule.ClassScheduleID,
                MaximumCapacity = schedule.MaximumCapacity,
                Country = schedule.Country,
                ClassName = schedule.ClassName
            };
            await _cache.SetData<ClassScheduleDto>($"classobj{schedule.ClassScheduleID}", classSj, schedule.EndTime);

            return booking;
        }

        public void CheckAndRefundWaitlistUsers()
        {
            var endClass = _context.ClassSchedules
                .AsNoTracking()
                .Include(x => x.Waitlists)
                .Where(x => x.EndTime < DateTime.Now && x.RefundDone == null)
                .ToList();
            if (endClass == null) return;


            foreach (var item in endClass)
            {
                if (item.Waitlists == null) continue;

                foreach (var item2 in item.Waitlists)
                {
                    var refundUP = _context.UserPackages.Find(item2.UserId);
                    if (refundUP == null) continue;
                    refundUP.RemainingCredits += item.RequiredCredits;

                    _context.UserPackages.Attach(refundUP);
                    _context.Entry(refundUP).State = EntityState.Modified;
                }
                _context.RemoveRange(item.Waitlists);
                item.RefundDone = true;
            }
            _context.ClassSchedules.UpdateRange(endClass);
            _context.SaveChanges();

        }

        public async Task<bool> DeleteBooking(int bid, int uid)
        {
            var booking = await _context.Bookings.FindAsync(bid);
            if (booking == null) throw new ApplicationException("Booking not found");


            if (booking.UserID != uid) throw new ApplicationException("You are not allowed to cancel this booking");


            var classStartTime = booking.ClassSchedule.StartTime;
            if (classStartTime - DateTime.Now > TimeSpan.FromHours(4))
            {
                var userPackage = await _context.UserPackages.Where(x => x.UserID == uid).FirstOrDefaultAsync();
                if (userPackage != null)
                {
                    userPackage.RemainingCredits += booking.ClassSchedule.RequiredCredits;
                    _context.UserPackages.Attach(userPackage);
                    _context.Entry(userPackage).State = EntityState.Modified;
                }
            }

            _context.Bookings.Remove(booking);

            var newBook = await _context.Waitlists
                .Where(x => x.ClassScheduleId == booking.ClassScheduleID)
                .OrderBy(x => x.WaitlistTime)
                .FirstOrDefaultAsync();
            if (newBook != null)
            {
                var bookEntity = new Booking
                {
                    UserID = uid,
                    ClassScheduleID = booking.ClassScheduleID,
                    BookingTime = DateTime.Now,
                };
                await _context.Bookings.AddAsync(bookEntity);
                _context.Waitlists.Remove(newBook);
            }


            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<Booking> GetBooking(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<ClassSchedule> GetClassSchedule(int id)
        {
            return await _context.ClassSchedules.FindAsync(id);
        }

        public async Task<List<ClassSchedule>> GetClassSchedules()
        {
            return await _context.ClassSchedules.AsNoTracking().Where(x => x.MaximumCapacity > 0 && x.RefundDone == null).ToListAsync();
        }

        //public async Task<Booking> UpdateBooking(Booking booking)
        //{
        //    var existingBooking = await _context.Bookings.FindAsync(booking.BookingID);
        //    if (existingBooking != null) return null;
        //    existingBooking.ClassScheduleID = booking.ClassScheduleID;
        //    existingBooking.BookingTime = booking.BookingTime;
        //    await _context.SaveChangesAsync();

        //    _cache.SetData(existingBooking.BookingID.ToString(), existingBooking, DateTimeOffset.Now.AddDays(1));
        //    return booking;
        //}
    }
}
