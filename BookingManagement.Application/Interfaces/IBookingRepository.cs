using BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingManagement.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task AddAsync(Booking booking);

        Task<Booking?> GetByIdAsync(Guid id);

        Task<IEnumerable<Booking>> GetAllAsync();

        Task UpdateAsync(Booking booking, byte[] rowVersion);

        Task SaveChangesAsync();

        Task<Booking?> GetOverlappingBookingAsync(
        string resourceId,
        DateTime startDateTime,
        DateTime endDateTime);

        Task<IEnumerable<Booking>> GetBookingsByFilterAsync(
            string resourceId,
            DateTime? startDateTime,
            DateTime? endDateTime);
    }
}
