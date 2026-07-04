using BookingManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingManagement.Application.DTOs.Bookings
{
    public class BookingResponse
    {
        public Guid Id { get; set; }

        public string ResourceId { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public BookingStatus Status { get; set; }

        public string RowVersion { get; set; } = string.Empty;

    }
}
