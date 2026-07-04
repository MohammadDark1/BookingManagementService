

namespace BookingManagement.Application.DTOs.Bookings
{
    public class CreateBookingRequest
    {
        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;  
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
    }
}
