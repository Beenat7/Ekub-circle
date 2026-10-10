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
    private readonly IRoundLifecycleStore _store;

    public CreatePayoutCommandHandler(IRoundLifecycleStore store)
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

        var addResult = await _store.AddPayoutAndAdvanceRoundAsync(payout, cancellationToken);
        if (!addResult.IsSuccess)
        {
            return Result<PayoutResponse>.Failure(addResult.Error!, addResult.StatusCode);
        }

        var res = new PayoutResponse(
            addResult.Value!.Id,
            addResult.Value.CircleId,
            addResult.Value.RoundId,
            addResult.Value.MemberId,
            addResult.Value.Amount,
            addResult.Value.PaidAt,
            "completed"
        );

        return Result<PayoutResponse>.Success(res, StatusCodes.Status201Created);
    }
}
