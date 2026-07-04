

namespace BookingManagement.Application.DTOs.Bookings
{
    public class BookingFilterRequest
    {
        public string ResourceId { get; set; } = string.Empty;

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
