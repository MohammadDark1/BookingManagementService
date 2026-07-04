using System.Net;
using System.Net.Http.Json;
using BookingManagement.Application.DTOs.Bookings;
using Xunit;

namespace BookingManagement.IntegrationTests
{
    public class BookingApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public BookingApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateBooking_ShouldReturnCreated()
        {
            // Arrange
            var request = new CreateBookingRequest
            {
                ResourceId = "Room-101",
                UserId = "Mohammad",
                StartDateTime = DateTime.UtcNow.AddHours(1),
                EndDateTime = DateTime.UtcNow.AddHours(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/Booking/CreateBooking",
                request);

            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var booking =
                await response.Content.ReadFromJsonAsync<BookingResponse>();

            Assert.NotNull(booking);
            Assert.Equal(request.ResourceId, booking!.ResourceId);
            Assert.Equal(request.UserId, booking.UserId);
        }

        [Fact]
public async Task GetBookings_ShouldReturnBookings()
{
    // Arrange
    var request = new CreateBookingRequest
    {
        ResourceId = "Room-200",
        UserId = "Mohammad",
        StartDateTime = DateTime.UtcNow.AddHours(1),
        EndDateTime = DateTime.UtcNow.AddHours(2)
    };

    await _client.PostAsJsonAsync(
        "/api/Booking/CreateBooking",
        request);

    // Act
    var response = await _client.GetAsync(
        "/api/Booking/GetBookings?resourceId=Room-200");

    // Assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var bookings = await response.Content
        .ReadFromJsonAsync<List<BookingResponse>>();

    Assert.NotNull(bookings);
    Assert.Single(bookings);
    Assert.Equal("Room-200", bookings![0].ResourceId);
    Assert.Equal("Mohammad", bookings[0].UserId);
}

[Fact]
public async Task DuplicateBooking_ShouldReturnConflict()
{
    // Arrange
    var request = new CreateBookingRequest
    {
        ResourceId = "Room-400",
        UserId = "Mohammad",
        StartDateTime = DateTime.UtcNow.AddHours(1),
        EndDateTime = DateTime.UtcNow.AddHours(2)
    };

    await _client.PostAsJsonAsync(
        "/api/Booking/CreateBooking",
        request);

    // Act
    var response = await _client.PostAsJsonAsync(
        "/api/Booking/CreateBooking",
        request);

    var content = await response.Content.ReadAsStringAsync();

    Console.WriteLine(content);
    
    // Assert
    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
}
    }
}