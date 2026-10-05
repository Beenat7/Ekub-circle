using MediatR;
using Microsoft.AspNetCore.Http;
using Ekub.Application.Common;
using Ekub.Application.Common.Persistence;
using Ekub.Application.Payouts.DTOs;
using Ekub.Domain.Entities;

namespace Ekub.Application.Payouts.Commands;

public sealed record CreatePayoutCommand(CreatePayoutRequest Request) : IRequest<Result<PayoutResponse>>;

public sealed class CreatePayoutCommandHandler : IRequestHandler<CreatePayoutCommand, Result<PayoutResponse>>
{
    private readonly IPayoutStore _store;

    public CreatePayoutCommandHandler(IPayoutStore store)
    {
        _store = store;
    }

    public async Task<Result<PayoutResponse>> Handle(CreatePayoutCommand command, CancellationToken cancellationToken)
    {
        var req = command.Request;

        var payout = new Payout
        {
            CircleId = req.CircleId,
            RoundId = req.RoundId,
            MemberId = req.MemberId,
            Amount = req.Amount,
            PaidAt = DateTime.UtcNow,
            RecordedBy = req.RecordedBy
        };

        await _store.AddAsync(payout, cancellationToken);

        var res = new PayoutResponse(
            payout.Id,
            payout.CircleId,
            payout.RoundId,
            payout.MemberId,
            payout.Amount,
            payout.PaidAt,
            "completed"
        );

        return Result<PayoutResponse>.Success(res, StatusCodes.Status201Created);
    }
}
