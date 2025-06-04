using System.Text.Json;
using System.Text;

namespace Melotrade.Services
{
    // Services/YocoPaymentService.cs
    public class YocoPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public YocoPaymentService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> CreateCheckoutAsync(decimal amount, string currency, string customerName,
            string description, string redirectUrl, string email, string reference)
        {
            var client = _httpClientFactory.CreateClient("Yoco");

            var payload = new
            {
                amount = (int)(amount * 100), // Convert to cents
                currency,
                name = customerName,
                description,
                redirectUrl,
                email,
                reference
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/checkouts", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Yoco API error: {response.StatusCode} - {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
            return result.RootElement.GetProperty("redirectUrl").GetString();
        }
    }
}
