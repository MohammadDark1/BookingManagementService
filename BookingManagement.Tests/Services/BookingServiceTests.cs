using BookingManagement.Application.DTOs.Bookings;
using BookingManagement.Application.Interfaces;
using BookingManagement.Application.Services;
using BookingManagement.Domain.Entities;
using BookingManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BookingManagement.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _bookingService = new BookingService(_bookingRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateBooking_WhenNoOverlap()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ResourceId = "Room-101",
                UserId = "Mohammad",
                StartDateTime = DateTime.UtcNow.AddHours(1),
                EndDateTime = DateTime.UtcNow.AddHours(2)
            };

            _bookingRepositoryMock
                .Setup(x => x.GetOverlappingBookingAsync(
                    request.ResourceId,
                    request.StartDateTime,
                    request.EndDateTime))
                .ReturnsAsync((Booking?)null);

            _bookingRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Booking>()))
                .Returns(Task.CompletedTask);

            _bookingRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookingService.CreateBookingAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.ResourceId, result.ResourceId);
            Assert.Equal(request.UserId, result.UserId);
            Assert.Equal(BookingStatus.Active, result.Status);

            _bookingRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Booking>()),
                Times.Once);

            _bookingRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrow_WhenBookingOverlaps()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ResourceId = "Room-101",
                UserId = "Mohammad",
                StartDateTime = DateTime.UtcNow.AddHours(1),
                EndDateTime = DateTime.UtcNow.AddHours(2)
            };

            var existingBooking = new Booking
            {
                Id = Guid.NewGuid(),
                ResourceId = "Room-101",
                UserId = "Ahmed",
                StartDateTime = request.StartDateTime.AddMinutes(-30),
                EndDateTime = request.EndDateTime.AddMinutes(30),
                Status = BookingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _bookingRepositoryMock
                .Setup(x => x.GetOverlappingBookingAsync(
                    request.ResourceId,
                    request.StartDateTime,
                    request.EndDateTime))
                .ReturnsAsync(existingBooking);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookingService.CreateBookingAsync(request));

            _bookingRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<Booking>()),
                Times.Never);

            _bookingRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldCancelBooking()
        {
            // Arrange
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                ResourceId = "Room-101",
                UserId = "Mohammad",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Status = BookingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var request = new CancelBookingRequest
            {
                RowVersion = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 })
            };

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(booking.Id))
                .ReturnsAsync(booking);

            _bookingRepositoryMock
                .Setup(x => x.UpdateAsync(
                    It.IsAny<Booking>(),
                    It.IsAny<byte[]>()))
                .Returns(Task.CompletedTask);

            _bookingRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _bookingService.CancelBookingAsync(booking.Id, request);

            // Assert
            Assert.Equal(BookingStatus.Cancelled, booking.Status);
            Assert.NotNull(booking.CancelledAt);

            _bookingRepositoryMock.Verify(
                x => x.UpdateAsync(
                    booking,
                    It.IsAny<byte[]>()),
                Times.Once);

            _bookingRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrow_WhenBookingNotFound()
        {
            // Arrange
            var bookingId = Guid.NewGuid();

            var request = new CancelBookingRequest
            {
                RowVersion = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 })
            };

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(bookingId))
                .ReturnsAsync((Booking?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _bookingService.CancelBookingAsync(bookingId, request));

            _bookingRepositoryMock.Verify(
                x => x.UpdateAsync(
                    It.IsAny<Booking>(),
                    It.IsAny<byte[]>()),
                Times.Never);

            _bookingRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldThrow_WhenConcurrencyConflictOccurs()
        {
            // Arrange
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                ResourceId = "Room-101",
                UserId = "Mohammad",
                Status = BookingStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var request = new CancelBookingRequest
            {
                RowVersion = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 })
            };

            _bookingRepositoryMock
                .Setup(x => x.GetByIdAsync(booking.Id))
                .ReturnsAsync(booking);

            _bookingRepositoryMock
                .Setup(x => x.UpdateAsync(
                    It.IsAny<Booking>(),
                    It.IsAny<byte[]>()))
                .Returns(Task.CompletedTask);

            _bookingRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookingService.CancelBookingAsync(booking.Id, request));

            Assert.Equal(
                "The booking was modified by another user.",
                ex.Message);
        }

        [Fact]
        public async Task GetBookingsByFilterAsync_ShouldReturnFilteredBookings()
        {
            // Arrange
            var filter = new BookingFilterRequest
            {
                ResourceId = "Room-101"
            };

            var bookings = new List<Booking>
            {
                new Booking
                {
                    Id = Guid.NewGuid(),
                    ResourceId = "Room-101",
                    UserId = "Mohammad",
                    StartDateTime = DateTime.UtcNow,
                    EndDateTime = DateTime.UtcNow.AddHours(1),
                    Status = BookingStatus.Active,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _bookingRepositoryMock
                .Setup(x => x.GetBookingsByFilterAsync(
                    filter.ResourceId,
                    filter.From,
                    filter.To))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetBookingsByFilterAsync(filter);

            // Assert
            Assert.Single(result);
            Assert.Equal("Room-101", result.First().ResourceId);
        }
    }
}