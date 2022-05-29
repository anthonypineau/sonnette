using Microsoft.AspNetCore.SignalR;

namespace sonnette.rtc.Hubs
{
    public class ChatHub : Hub
    {
        public static bool nePasDeranger = false;
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message+"");
        }

        public async Task sendNePasDeranger(string message)
        {
            if(message=="true")
                nePasDeranger = true;
            else
                nePasDeranger = false;
            await Clients.All.SendAsync("setvalue", message);
        }
    }
}
