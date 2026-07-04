using BookingManagement.Application.DTOs.Bookings;
using BookingManagement.Application.Interfaces;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            if (request.StartDateTime >= request.EndDateTime)
            {
                throw new ArgumentException("StartDateTime must be earlier than EndDateTime.");
            }

            var overlappingBooking = await _bookingRepository.GetOverlappingBookingAsync(
                request.ResourceId,
                request.StartDateTime,
                request.EndDateTime);

            if (overlappingBooking != null)
            {
                throw new InvalidOperationException("The resource is already booked during the requested time.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                ResourceId = request.ResourceId,
                UserId = request.UserId,
                StartDateTime = request.StartDateTime,
                EndDateTime = request.EndDateTime,
                Status = BookingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveChangesAsync();

            return new BookingResponse
            {
                Id = booking.Id,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                Status = booking.Status,
                RowVersion = booking.RowVersion != null
                    ? Convert.ToBase64String(booking.RowVersion)
                    : string.Empty
            };
        }

        public async Task<IEnumerable<BookingResponse>> GetBookingsByFilterAsync(BookingFilterRequest filterRequest)
        {
            var bookings = await _bookingRepository.GetBookingsByFilterAsync(
            filterRequest.ResourceId,
            filterRequest.From,
            filterRequest.To);

            return bookings.Select(booking => new BookingResponse
            {
                Id = booking.Id,
                ResourceId = booking.ResourceId,
                UserId = booking.UserId,
                StartDateTime = booking.StartDateTime,
                EndDateTime = booking.EndDateTime,
                Status = booking.Status,
                RowVersion = booking.RowVersion != null
                    ? Convert.ToBase64String(booking.RowVersion)
                    : string.Empty
            });
        }

        public async Task CancelBookingAsync(Guid bookingId, CancelBookingRequest request)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new InvalidOperationException("Booking is already cancelled.");
            }

            var rowVersion = Convert.FromBase64String(request.RowVersion);

            booking.Status = BookingStatus.Cancelled;
            booking.CancelledAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking, rowVersion); ;

            try
            {
                await _bookingRepository.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "The booking was modified by another user.");
            }
        }
    }
}