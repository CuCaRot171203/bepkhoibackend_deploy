using Microsoft.AspNetCore.SignalR;

namespace BepKhoiBackend.API.Hubs
{
    public class SignalrHub : Hub
    {
        // Tham gia group (client gọi khi load component)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        // Rời khỏi group (client gọi khi rời component)
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Gửi sự kiện tới group tương ứng (backend gọi)
        public async Task SendUpdateToGroup(string groupName, int entityId)
        {
            await Clients.Group(groupName).SendAsync($"Receive{groupName}Update", entityId);
        }
    }
}
