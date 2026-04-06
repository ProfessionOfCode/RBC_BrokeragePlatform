using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using RBC.BrokeragePlatform.WebAPI.Hubs;

namespace RBC.BrokeragePlatform.WebAPI.Tests.Hubs
{
    public class BrokerageHubTests
    {
        [Fact]
        public async Task SubscribeToPositionUpdates_AddsClientToGroup()
        {
            var mockLogger = new Mock<ILogger<BrokerageHub>>();
            var mockGroups = new Mock<IGroupManager>();
            var mockContext = new Mock<HubCallerContext>();
            var hub = new BrokerageHub(mockLogger.Object);
            var connectionId = "conn1";
            mockContext.SetupGet(c => c.ConnectionId).Returns(connectionId);
            hub.Context = mockContext.Object;
            hub.Groups = mockGroups.Object;

            await hub.SubscribeToPositionUpdates(1);

            mockGroups.Verify(g => g.AddToGroupAsync(connectionId, "account_positions_1", default), Times.Once);
        }

        [Fact]
        public async Task UnsubscribeFromPositionUpdates_RemovesClientFromGroup()
        {
            var mockLogger = new Mock<ILogger<BrokerageHub>>();
            var mockGroups = new Mock<IGroupManager>();
            var mockContext = new Mock<HubCallerContext>();
            var hub = new BrokerageHub(mockLogger.Object);
            var connectionId = "conn2";
            mockContext.SetupGet(c => c.ConnectionId).Returns(connectionId);
            hub.Context = mockContext.Object;
            hub.Groups = mockGroups.Object;

            await hub.UnsubscribeFromPositionUpdates(2);

            mockGroups.Verify(g => g.RemoveFromGroupAsync(connectionId, "account_positions_2", default), Times.Once);
        }
    }
}
