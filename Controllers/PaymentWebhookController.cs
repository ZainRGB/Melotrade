using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/payment/webhook")]
public class PaymentWebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] JsonElement payload)
    {
        // TODO: Save or process webhook data here
        Console.WriteLine($"Received webhook payload: {payload}");

        return Ok(); // Acknowledge to Yoco that webhook was received
    }
}




//using Microsoft.AspNetCore.Mvc;

//namespace Melotrade.Controllers
//{
//    public class PaymentWebhookController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}
