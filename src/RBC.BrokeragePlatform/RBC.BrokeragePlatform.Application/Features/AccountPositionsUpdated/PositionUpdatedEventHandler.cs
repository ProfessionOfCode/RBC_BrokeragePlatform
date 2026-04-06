using MediatR;
using Microsoft.Extensions.Logging;
using RBC.BrokeragePlatform.Application.Interfaces.UnitOfWorks;

namespace RBC.BrokeragePlatform.Application.Features.AccountPositionsUpdated
{
    public class PositionUpdatedEventHandler : INotificationHandler<PositionUpdatedEvent>
    {
        private readonly IPositionUnitOfWork _positionUnitOfWork;
        private readonly ILogger<PositionUpdatedEventHandler> _logger;

        public PositionUpdatedEventHandler(IPositionUnitOfWork positionUnitOfWork, ILogger<PositionUpdatedEventHandler> logger)
        {
            _positionUnitOfWork = positionUnitOfWork;
            _logger = logger;
        }

        public async Task Handle(PositionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling PositionUpdatedEvent started - Account Id: {AccountId}", notification.AccountId);            

            try
            {
                if(notification.PriceChange == 0)
                {
                    _logger.LogInformation("No price change detected for Account Id: {AccountId}. Skipping position update.", notification.AccountId);
                    return;
                }

                _positionUnitOfWork.BeginTransaction();

                var positionsUpdatedCount = await _positionUnitOfWork.UpdatePositionsPriceAsync(notification.AccountId, notification.PriceChange, cancellationToken);

                _positionUnitOfWork.SaveChanges();

                _positionUnitOfWork.CommitTransaction();

                _logger.LogInformation("Handling PositionUpdatedEvent completed successfully - Account Id: {AccountId}, Position updated count: {PositionCount}",
                    notification.AccountId, positionsUpdatedCount);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error during PositionUpdatedEvent handling - Account Id: {AccountId}, Exception: {ExceptionMessage}",
                    notification.AccountId, ex.Message);
            }

        }
    }
}
