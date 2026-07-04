using BookingManagement.Domain.Enums;


namespace BookingManagement.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CancelledAt { get; set; }

        public byte[]? RowVersion { get; set; }
    }
}
