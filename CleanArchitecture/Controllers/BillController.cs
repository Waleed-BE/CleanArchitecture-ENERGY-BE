using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CleanArchitecture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BillController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("analyze-bill-Electricity")]
        public async Task<IActionResult> AnalyzeElectricityBill(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Read file and convert it to base64
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var fileBase64 = Convert.ToBase64String(fileBytes);

            // Determine file type (PDF or Image)
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var mimeType = file.ContentType.ToLower();

            string promptText = "Extract the units of electricity consumed, and total bill from this electricity bill and price per unit that is calculated exluding the VAT and only decimal value but converted to string no other thing like per kwh. return data in json format like units consumed and total bill and price per unit";
            object requestBody = null;

            // If PDF file, handle as PDF
            if (fileExtension == ".pdf" || mimeType == "application/pdf")
            {
                requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = promptText },
                                new
                                {
                                    inline_data = new
                                    {
                                        mime_type = "application/pdf",
                                        data = fileBase64
                                    }
                                }
                            }
                        }
                    }
                };
            }
            // If Image file, handle as Image
            else if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || mimeType.StartsWith("image/"))
            {
                requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = promptText },
                                new
                                {
                                    inline_data = new
                                    {
                                        mime_type = "image/jpeg", // You can change this based on the file extension (jpg, png, etc.)
                                        data = fileBase64
                                    }
                                }
                            }
                        }
                    }
                };
            }
            else
            {
                return BadRequest("Unsupported file type.");
            }

            var client = _httpClientFactory.CreateClient();
            var apiKey = _configuration["Gemini:APIKey"];
            var requestJson = JsonConvert.SerializeObject(requestBody);

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-pro:generateContent?key={apiKey}")
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Step 1: Parse the response content as JSON
            using var document = JsonDocument.Parse(responseContent);

            // Step 2: Navigate to the parts.text field
            var text = document
                .RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            // Step 3: Clean up the text to remove code block formatting
            var jsonText = Regex.Replace(text, @"```json|```", "").Trim();

            // Step 4: Parse the inner JSON
            using var innerDoc = JsonDocument.Parse(jsonText);

            var unitsConsumed = innerDoc.RootElement.GetProperty("units_consumed").GetString();
            var totalBill = innerDoc.RootElement.GetProperty("total_bill").GetString();
            var pricePerUnit = innerDoc.RootElement.GetProperty("price_per_unit").GetString();
            var result = new
            {
                UnitsConsumed = unitsConsumed,
                TotalBill = totalBill,
                PricePerUnit = pricePerUnit
            };

            return Ok(result);
        }
    }
}
