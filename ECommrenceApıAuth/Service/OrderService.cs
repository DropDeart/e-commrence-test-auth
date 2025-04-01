// Services/OrderBackgroundService.cs
using ECommrenceApıAuth.Domain;
using ECommrenceApıAuth.Interface;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

public class OrderBackgroundService : BackgroundService
{
    private readonly IAuthService _authService;
    private readonly HttpClient _httpClient;
    private readonly ExternalApiSettings _settings;
    private readonly ILogger<OrderBackgroundService> _logger;

    public OrderBackgroundService(
        IAuthService authService,
        HttpClient httpClient,
        IOptions<ExternalApiSettings> settings,
        ILogger<OrderBackgroundService> logger)
    {
        _authService = authService;
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Fetching orders at {Time}", DateTime.UtcNow);

                var token = await _authService.GetValidAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(_settings.ApiUrl, stoppingToken);
                response.EnsureSuccessStatusCode();

                var orders = await response.Content.ReadFromJsonAsync<List<Order>>(); //Order domain modelini de eklemek istemedim. :)
                _logger.LogInformation("Retrieved {Count} orders", orders?.Count);
            }
            catch (RateLimitException ex)
            {
                _logger.LogError(ex, "Rate limit exceeded. Next try after 1 hour.");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}