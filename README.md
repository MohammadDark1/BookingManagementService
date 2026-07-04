# Booking Management Service

## Project Overview

This project implements a Booking Management Service that allows users to create, retrieve, and cancel bookings for shared resources such as meeting rooms.

The system prevents overlapping bookings for the same resource and includes a simple React frontend that interacts with the ASP.NET Core Web API.

---

# API Documentation

## Create Booking

**POST** `/api/Booking/CreateBooking`

Creates a new booking.

---

## Get Bookings

**GET** `/api/Booking/GetBookings`

Returns bookings for a specific resource with optional date-range filtering.

Example:

```
GET /api/Booking/GetBookings?resourceId=Room-101
```

---

## Cancel Booking

**DELETE** `/api/Booking/{id}`

Cancels an existing booking using optimistic concurrency by providing the booking RowVersion.

---

# Assumptions

The following assumptions were made during the implementation:

- All dates are stored in UTC.
- A booking contains a ResourceId, UserId, StartDateTime, EndDateTime, Status, CreatedAt, CancelledAt, and RowVersion.
- Cancelled bookings do not block future bookings.
- A booking ending exactly when another booking starts is **not** considered overlapping.
- Cancelling a booking is implemented as a soft delete by changing its status to **Cancelled**.

---

# Design Write-up

## A. How did you define and enforce overlapping bookings, and why?

Two bookings are considered overlapping if they reserve the same resource and their time intervals intersect.

The overlap rule is implemented using the following condition:

```
StartDateTime < ExistingBooking.EndDateTime &&
EndDateTime > ExistingBooking.StartDateTime
```

This definition correctly allows one booking to start exactly when another booking ends because their time intervals do not overlap.

Overlap validation is performed before creating a booking, and only active bookings are checked.

---

## B. What did you assume about concurrency?

I implemented optimistic concurrency using SQL Server's RowVersion feature.

Each booking stores a RowVersion value that is returned to the client. When cancelling a booking, the client sends the RowVersion back to the API.

Entity Framework Core compares the original RowVersion with the current database value before saving changes. If another user has modified the booking first, the update fails with a concurrency exception, preventing conflicting updates.

---

## C. What would break in your design at scale, and where would the first bottleneck be?

The first bottleneck would be the database queries used for overlap detection as the number of bookings grows.

Searching for overlapping bookings becomes more expensive with large datasets.

To improve scalability, I would:

- Add proper indexes.
- Introduce pagination for retrieval endpoints.
- Optimize overlap queries.
- Consider caching frequently accessed data.

---

## D. How would you evolve this into a distributed system?

To support a distributed environment, I would:

- Split the application into independent services.
- Introduce asynchronous communication using a message broker.
- Add centralized logging and monitoring.
- Use distributed caching.
- Consider distributed locking or transactional messaging where stronger consistency is required.

---

## E. Which tradeoff did you prioritize — simplicity, correctness, or performance — and why?

I prioritized **correctness**.

A booking system must guarantee that bookings remain consistent and valid. Preventing invalid bookings and maintaining data integrity is more important than maximizing performance in this implementation.

---

# Extension Task

### Selected Option

**Option 1 – Concurrency**

I chose the concurrency extension because booking systems commonly face concurrent update scenarios.

The implementation uses optimistic concurrency with SQL Server RowVersion.

When a booking is cancelled, Entity Framework Core verifies that the RowVersion supplied by the client matches the current RowVersion stored in the database. If another update has already modified the booking, the operation fails with a concurrency exception instead of overwriting the latest data.

This approach avoids unnecessary database locking while still protecting data consistency during concurrent updates.

---

# Testing

The project includes automated tests covering the core functionality.

### Unit Tests

The unit tests verify:

- Successful booking creation.
- Prevention of overlapping bookings.
- Successful booking cancellation.
- Booking not found scenarios.
- Optimistic concurrency conflicts.
- Booking retrieval with filters.

### Integration Tests

The integration tests verify the API behavior end-to-end, including:

- Creating a booking.
- Preventing duplicate bookings.
- Cancelling a booking.
- Testing optimistic concurrency during cancellation.
