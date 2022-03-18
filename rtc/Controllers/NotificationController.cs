using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using sonnette.rtc.Hubs;
using sonnette.rtc.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace sonnette.rtc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public NotificationController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // POST api/<SonetteController>
        [HttpPost]
        public void Post([FromBody] Sonnette value)
        {
            _hubContext.Clients.All.SendAsync("ReceiveMessage", "test", value.Id+" "+value.Date+" "+value.TypeAppui);
        }
    }
}
