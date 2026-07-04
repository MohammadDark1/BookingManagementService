using BookingManagement.Application.DTOs.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingManagement.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);
        Task<IEnumerable<BookingResponse>> GetBookingsByFilterAsync(BookingFilterRequest filterRequest);
        Task CancelBookingAsync(Guid bookingId, CancelBookingRequest request);
    }
}
