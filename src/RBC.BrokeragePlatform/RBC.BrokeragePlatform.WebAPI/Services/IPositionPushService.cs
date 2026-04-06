using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.WebAPI.Services
{
    public interface IPositionPushService
    {
        Task PushAccountPositionsAsync(int accountId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PositionDto>> GetAccountPositionsByIdAsync(int accountId, CancellationToken cancellationToken = default);
    }
}
