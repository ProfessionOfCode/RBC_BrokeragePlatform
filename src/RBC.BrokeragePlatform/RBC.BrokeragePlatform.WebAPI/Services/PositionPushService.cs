using MediatR;
using Microsoft.AspNetCore.SignalR;
using RBC.BrokeragePlatform.Application.Features.GetAccountPositions;
using RBC.BrokeragePlatform.SharedCore.DTOs;
using RBC.BrokeragePlatform.WebAPI.Hubs;

namespace RBC.BrokeragePlatform.WebAPI.Services
{
    public class PositionPushService : IPositionPushService
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<BrokerageHub> _hubContext;
        private readonly ILogger<PositionPushService> _logger;
        private readonly Dictionary<int, IEnumerable<PositionDto>> _positionCache;

        public PositionPushService(IMediator mediator, IHubContext<BrokerageHub> hubContext, ILogger<PositionPushService> logger)
        {
            _mediator = mediator;
            _hubContext = hubContext;
            _logger = logger;
            _positionCache = new Dictionary<int, IEnumerable<PositionDto>>();
        }

        public async Task<IEnumerable<PositionDto>> GetAccountPositionsByIdAsync(int accountId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving positions for account - AccountId: {AccountId}", accountId);

            try
            {
                var query = new GetAccountPositionsQuery(accountId);
                var positions = await _mediator.Send(query, cancellationToken);

                _logger.LogInformation("Positions retrieved successfully - AccountId: {AccountId}, PositionCount: {PositionCount}", 
                    accountId, positions.Count());

                return positions;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error retrieving positions - AccountId: {AccountId}, Exception: {ExceptionMessage}", 
                    accountId, ex.Message);
                throw;
            }
        }

        public async Task PushAccountPositionsAsync(int accountId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting position push - AccountId: {AccountId}", accountId);

            try
            {
                var currentPositions = await GetAccountPositionsByIdAsync(accountId, cancellationToken);
                var currentPositionsList = currentPositions.ToList();

                var hasChanged = DetectPositionChanges(accountId, currentPositionsList);

                if (hasChanged)
                {
                    _logger.LogInformation("Position changes detected - AccountId: {AccountId}, PositionCount: {PositionCount}", 
                        accountId, currentPositionsList.Count);

                    await _hubContext.Clients.All                       
                        .SendAsync("PositionUpdated", currentPositionsList, cancellationToken);

                    _logger.LogInformation("Position update pushed to clients - AccountId: {AccountId}", accountId);
                }
                else
                {
                    _logger.LogInformation("No position changes detected - AccountId: {AccountId}", accountId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error during position push - AccountId: {AccountId}, Exception: {ExceptionMessage}", 
                    accountId, ex.Message);
            }
        }

        private bool DetectPositionChanges(int accountId, List<PositionDto> newPositions)
        {
            if (!_positionCache.ContainsKey(accountId))
            {
                _positionCache[accountId] = newPositions;
                _logger.LogInformation("Initial position cache set - AccountId: {AccountId}", accountId);
                return true;
            }

            var oldPositions = _positionCache[accountId].ToList();

            if (oldPositions.Count != newPositions.Count)
            {
                _logger.LogInformation("Position count changed - AccountId: {AccountId}, OldCount: {OldCount}, NewCount: {NewCount}", 
                    accountId, oldPositions.Count, newPositions.Count);
                _positionCache[accountId] = newPositions;
                return true;
            }

            var hasChanged = false;
            foreach (var newPos in newPositions)
            {
                var oldPos = oldPositions.FirstOrDefault(p => p.PositionId == newPos.PositionId);
                if (oldPos == null || !PositionsAreEqual(oldPos, newPos))
                {
                    _logger.LogInformation("Position change detected - AccountId: {AccountId}, Symbol: {Symbol}, OldQty: {OldQty}, NewQty: {NewQty}, OldCost: {OldCost}, NewCost: {NewCost}", 
                        accountId, newPos.Symbol, oldPos?.Quantity ?? 0, newPos.Quantity, oldPos?.AverageCostPerShare ?? 0, newPos.AverageCostPerShare);
                    hasChanged = true;
                    break;
                }
            }

            if (hasChanged)
            {
                _positionCache[accountId] = newPositions;
            }

            return hasChanged;
        }

        private bool PositionsAreEqual(PositionDto pos1, PositionDto pos2)
        {
            return pos1.PositionId == pos2.PositionId &&
                   pos1.AccountId == pos2.AccountId &&
                   pos1.EquityId == pos2.EquityId &&
                   pos1.Symbol == pos2.Symbol &&
                   pos1.Quantity == pos2.Quantity &&
                   pos1.AverageCostPerShare == pos2.AverageCostPerShare;
        }
    }
}
