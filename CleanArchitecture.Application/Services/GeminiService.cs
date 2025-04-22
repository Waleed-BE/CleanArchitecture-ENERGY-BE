using CleanArchitecture.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using System;
using CleanArchitecture.Application.Features.Business.UserExpense.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CleanArchitecture.Application.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        public GeminiService(IConfiguration configuration, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
            _httpClient = new HttpClient();
            _apiKey = _configuration["Gemini:APIKey"];
        }
        public async Task<string> GenerateContentAsync(Guid UserId, string text)
        {
            var userId = UserId.ToString();
            var expenses = await _mediator.Send(new GetUserExpensesQuery { UserId = userId });

            var groupedExpenses = expenses
                     .GroupBy(e => new { e.ExpenseTypeId, e.ExpenseTypeName })
                     .Select(g => new
                     {
                         ExpenseTypeId = g.Key.ExpenseTypeId,
                         ExpenseTypeName = g.Key.ExpenseTypeName,
                         TotalCost = g.Select(x => new
                         {
                             Quantity = x.Quantity,
                             Price = x.TotalCost * x.Quantity, // Assuming total cost already includes price * quantity
                             TotalCost = x.TotalCost,
                             CreatedForDate = x.ExpenseForDate
                         }).ToList()
                     })
                     .ToList();

            var jsonString = JsonSerializer.Serialize(groupedExpenses, new JsonSerializerOptions { WriteIndented = true });

            text = text + jsonString;
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text } } }
                }
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using var doc = JsonDocument.Parse(responseString);

                    string Analysis = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString()
                        .Trim();
                    return Analysis;
                }
                else
                {
                    return $"Error: {response.StatusCode}\n{responseString}";
                }
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }
    }
}
