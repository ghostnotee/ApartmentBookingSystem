using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Bookings.GetBookings;

public sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;