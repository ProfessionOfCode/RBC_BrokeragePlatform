using Microsoft.AspNetCore.SignalR;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.WebAPI.Hubs
{
    public class BrokerageHub : Hub
    {
        private readonly ILogger<BrokerageHub> _logger;
        private const string PositionGroupPrefix = "account_positions_";

        public BrokerageHub(ILogger<BrokerageHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected - ConnectionId: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected - ConnectionId: {ConnectionId}", Context.ConnectionId);
            if (exception != null)
            {
                _logger.LogInformation("Disconnection exception: {ExceptionMessage}", exception.Message);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SubscribeToPositionUpdates(int accountId)
        {
            var groupName = $"{PositionGroupPrefix}{accountId}";
            _logger.LogInformation("Client subscribing to position updates - ConnectionId: {ConnectionId}, AccountId: {AccountId}, GroupName: {GroupName}", 
                Context.ConnectionId, accountId, groupName);
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            _logger.LogInformation("Client successfully added to group - ConnectionId: {ConnectionId}, GroupName: {GroupName}", 
                Context.ConnectionId, groupName);
        }

        public async Task UnsubscribeFromPositionUpdates(int accountId)
        {
            var groupName = $"{PositionGroupPrefix}{accountId}";
            _logger.LogInformation("Client unsubscribing from position updates - ConnectionId: {ConnectionId}, AccountId: {AccountId}", 
                Context.ConnectionId, accountId);
            
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            _logger.LogInformation("Client successfully removed from group - ConnectionId: {ConnectionId}, GroupName: {GroupName}", 
                Context.ConnectionId, groupName);
        }

        public async Task SendPositionUpdatesToGroup(int accountId, IEnumerable<PositionDto> positions)
        {
            var groupName = $"{PositionGroupPrefix}{accountId}";
            var positionCount = positions.Count();
            
            _logger.LogInformation("Broadcasting position updates to group - GroupName: {GroupName}, PositionCount: {PositionCount}", 
                groupName, positionCount);
            
            await Clients.Group(groupName).SendAsync("PositionUpdated", positions);
            
            _logger.LogInformation("Position updates broadcast completed - GroupName: {GroupName}", groupName);
        }
    }
}
