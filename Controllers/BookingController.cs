using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBus.Models;

namespace SmartBus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public BookingController(ApplicationDbContext context, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromBody] BookingDto bookingDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var schedule = await _context.Schedules
                    .Include(s => s.Bus)
                    .Include(s => s.Route)
                    .FirstOrDefaultAsync(s => s.ScheduleId == bookingDto.ScheduleId);

                if (schedule == null)
                    return NotFound("Schedule not found");

                if (schedule.AvailableSeats < bookingDto.NumberOfSeats)
                    return BadRequest("Not enough seats available");

                var user = await _context.Users.FindAsync(bookingDto.UserId);
                if (user == null)
                    return NotFound("User not found");

                var booking = new Booking
                {
                    UserId = bookingDto.UserId,
                    ScheduleId = bookingDto.ScheduleId,
                    BookingDate = DateTime.UtcNow,
                    NumberOfSeats = bookingDto.NumberOfSeats,
                    TotalFare = schedule.Fare * bookingDto.NumberOfSeats,
                    Status = "Confirmed",
                    PaymentStatus = "Paid"
                };

                _context.Bookings.Add(booking);
                schedule.AvailableSeats -= bookingDto.NumberOfSeats;

                await _context.SaveChangesAsync();

                // Send confirmation email
                var emailDto = new EmailDto
                {
                    To = user.Email,
                    Subject = "Booking Confirmation - SmartBus",
                    Body = $@"<h2>Booking Confirmation</h2>
                            <p>Dear {user.Name},</p>
                            <p>Your booking has been confirmed with the following details:</p>
                            <p>Booking ID: {booking.BookingId}</p>
                            <p>Route: {schedule.Route.Source} to {schedule.Route.Destination}</p>
                            <p>Date: {schedule.DepartureTime.ToString("dd MMM yyyy")}</p>
                            <p>Time: {schedule.DepartureTime.ToString("HH:mm")}</p>
                            <p>Number of Seats: {booking.NumberOfSeats}</p>
                            <p>Total Fare: ${booking.TotalFare}</p>"
                };

                _emailService.SendEmail(emailDto);

                await transaction.CommitAsync();
                return Ok(new { BookingId = booking.BookingId, Message = "Booking confirmed successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while processing your booking");
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromBody] CancellationDto cancellationDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Schedule)
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(b => b.BookingId == cancellationDto.BookingId);

                if (booking == null)
                    return NotFound("Booking not found");

                if (booking.Status == "Cancelled")
                    return BadRequest("Booking is already cancelled");

                // Check if cancellation is within claim time (e.g., 24 hours before departure)
                var claimTimeHours = int.Parse(_config.GetSection("CancellationClaimTimeHours").Value ?? "24");
                var isWithinClaimTime = booking.Schedule.DepartureTime > DateTime.UtcNow.AddHours(claimTimeHours);

                var refundAmount = isWithinClaimTime ? booking.TotalFare : 0;

                var cancellation = new Cancellation
                {
                    BookingId = booking.BookingId,
                    CancellationReason = cancellationDto.CancellationReason,
                    CancellationDate = DateTime.UtcNow,
                    RefundAmount = refundAmount,
                    RefundStatus = isWithinClaimTime ? "Approved" : "Not Eligible"
                };

                booking.Status = "Cancelled";
                booking.Schedule.AvailableSeats += booking.NumberOfSeats;

                _context.Cancellations.Add(cancellation);
                await _context.SaveChangesAsync();

                // Send cancellation email
                var emailDto = new EmailDto
                {
                    To = booking.User.Email,
                    Subject = "Booking Cancellation - SmartBus",
                    Body = $@"<h2>Booking Cancellation Confirmation</h2>
                            <p>Dear {booking.User.Name},</p>
                            <p>Your booking (ID: {booking.BookingId}) has been cancelled successfully.</p>
                            <p>Refund Status: {(isWithinClaimTime ? "Full refund approved" : "No refund applicable")}</p>
                            <p>Refund Amount: ${refundAmount}</p>"
                };

                _emailService.SendEmail(emailDto);

                await transaction.CommitAsync();
                return Ok(new
                {
                    Message = "Booking cancelled successfully",
                    RefundAmount = refundAmount,
                    RefundStatus = cancellation.RefundStatus
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while processing your cancellation");
            }
        }
    }
}
