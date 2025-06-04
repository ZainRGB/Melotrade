using Melotrade.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Melotrade.Controllers
{
    public class BuyItemController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public BuyItemController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }



        [HttpGet]
        public IActionResult Buy(int itemId)
        {
            var model = new BuyItemViewModel
            {
                ItemId = itemId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Buy(BuyItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // TODO: Save to database (you can use EF or raw SQL here)

            var yocoResponse = await CallYocoAPI();

            // Optionally parse JSON response here to extract checkout link
            return Content(yocoResponse); // Later, redirect to Yoco checkout URL
        }

        private async Task<string> CallYocoAPI()
        {
            var url = "https://payments.yoco.com/api/checkouts";
            var secretKey = "sk_live_6ec33a5c8BvJnV2c31a43aba1530";

            var payload = new
            {
                amount = 2000, // e.g. 20 ZAR in cents
                currency = "ZAR"
            };

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {secretKey}");
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
