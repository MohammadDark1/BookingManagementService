using BookingManagement.Application.Interfaces;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;
using BookingManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task UpdateAsync(Booking booking, byte[] rowVersion)
        {
            _context.Bookings.Update(booking);

            _context.Entry(booking)
                    .Property(x => x.RowVersion)
                    .OriginalValue = rowVersion;

            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Booking?> GetOverlappingBookingAsync(
            string resourceId,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(x =>
                    x.ResourceId == resourceId &&
                    x.Status == BookingStatus.Active &&
                    startDateTime < x.EndDateTime &&
                    endDateTime > x.StartDateTime);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByFilterAsync(
            string resourceId,
            DateTime? startDateTime,
            DateTime? endDateTime)
        {
            var query = _context.Bookings.AsQueryable();

            query = query.Where(x => x.ResourceId == resourceId);

            if (startDateTime.HasValue)
                query = query.Where(x => x.StartDateTime >= startDateTime.Value);

            if (endDateTime.HasValue)
                query = query.Where(x => x.EndDateTime <= endDateTime.Value);

            return await query
                .OrderBy(x => x.StartDateTime)
                .ToListAsync();
        }
    }
}