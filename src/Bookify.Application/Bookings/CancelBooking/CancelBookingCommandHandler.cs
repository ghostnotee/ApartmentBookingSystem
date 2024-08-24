﻿using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;

namespace Bookify.Application.Bookings.CancelBooking;

internal sealed class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CancelBookingCommandHandler(IDateTimeProvider dateTimeProvider, IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
    {
        _dateTimeProvider = dateTimeProvider;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId, cancellationToken);

        if (booking is null) return Result.Failure(BookingErrors.NotFound);

        var result = booking.Cancel(_dateTimeProvider.UtcNow);

        if (result.IsFailure) return result;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}