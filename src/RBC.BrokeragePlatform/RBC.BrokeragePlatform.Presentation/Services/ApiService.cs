using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using System.Net.Http;

namespace RBC.BrokeragePlatform.WPF.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2),
                    (outcome, timespan, retryAttempt, context) =>
                    {
                        _logger.LogWarning("Retry {RetryAttempt} for {Endpoint}", retryAttempt, context["endpoint"]);
                    });
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        /// <inheritdoc/>
        public async Task<string> GetAsync(string endpoint)
        {
            var client = _httpClientFactory.CreateClient(nameof(IApiService));
            var context = new Context { ["endpoint"] = endpoint };
            var response = await _retryPolicy.ExecuteAsync((ctx) => client.GetAsync(endpoint), context);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("GET {Endpoint} succeeded", endpoint);
            return content;
        }

        /// <inheritdoc/>
        public async Task<bool> PostAsync(string endpoint, object payload)
        {
            var client = _httpClientFactory.CreateClient(nameof(IApiService));
            var context = new Context { ["endpoint"] = endpoint };
            var response = await _retryPolicy.ExecuteAsync((ctx) =>
            client.PostAsync(endpoint, new StringContent(System.Text.Json.JsonSerializer.Serialize(payload),
                    System.Text.Encoding.UTF8, "application/json")), context);
            _logger.LogInformation("GET {Endpoint} succeeded", endpoint);
            return response.IsSuccessStatusCode;
        }
    }
}
