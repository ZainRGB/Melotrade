using Melotrade.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Melotrade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YocoWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<YocoWebhookController> _logger;

        public YocoWebhookController(ApplicationDbContext context, ILogger<YocoWebhookController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook([FromBody] YocoWebhookPayload payload)
        {
            try
            {
                // Verify the webhook signature if needed
                // (Yoco may send a signature header you should validate)

                _logger.LogInformation($"Received Yoco webhook for order {payload.Metadata["reference"]}");

                var orderNo = payload.Metadata["reference"];
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderNo == orderNo);

                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found for order {orderNo}");
                    return NotFound();
                }

                if (payload.Status == "succeeded")
                {
                    payment.Active = "Y";
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Payment {orderNo} marked as successful");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Yoco webhook");
                return StatusCode(500);
            }
        }
    }

    public class YocoWebhookPayload
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        // Add other properties as needed from Yoco's webhook docs
    }
}
