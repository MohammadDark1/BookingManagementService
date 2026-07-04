using Microsoft.AspNetCore.Mvc;
using BookingManagement.Application.DTOs.Bookings;
using BookingManagement.Application.Interfaces;

namespace BookingManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService) 
        { 
            _bookingService = bookingService;
        }

        [HttpPost("CreateBooking")]
        public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest request) 
        {
            var booking = await _bookingService.CreateBookingAsync(request);

            return CreatedAtAction(
                nameof(GetBookings),
                new {resourceId = booking.ResourceId},
                booking
                );
        }

        [HttpGet("GetBookings")]
        public async Task<ActionResult<IEnumerable<BookingResponse>>> GetBookings([FromQuery] BookingFilterRequest filterRequest)
        {
            var bookings = await _bookingService.GetBookingsByFilterAsync(filterRequest);

            return Ok(bookings);    
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> CancelBooking(
            Guid id,
            [FromBody] CancelBookingRequest request)
        {
            await _bookingService.CancelBookingAsync(id, request);
            return NoContent();
        }
    }
}
