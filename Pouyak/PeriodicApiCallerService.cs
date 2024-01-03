using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Pouyak
{
    public class PeriodicApiCallerService : IHostedService, IDisposable
    {
        private readonly ILogger<PeriodicApiCallerService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private Timer? _timer;
        public PeriodicApiCallerService(ILogger<PeriodicApiCallerService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var initialDelay = TimeSpan.Zero;
            var interval = TimeSpan.FromSeconds(10);
            _timer = new Timer(CallApis, null, initialDelay, interval);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private async void CallApis(object state)
        {
            var services = new ServiceCollection();
            var connectionString = "server=62.106.95.104;port=3306;database=talayieh;user=root;password=1372328$oheiL!@#$!;";
            services.AddDbContext<Context>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetService<Context>();
            try
            {
                string apiUrl = "https://smartmeter.paarian.com/api/scada/v1/flowMeter-manager/profile";
                string fromDate = "1402/10/12-00:00:00";
                string toDate = "1402/10/12-00:30:00";
                bool daily = true;
                bool cmUnit = true;
                string flowMeterSerial = "909801044439";
                string fullUrl = $"{apiUrl}?FromDate={fromDate}&Date={toDate}&Daily={daily}&CMUnit={cmUnit}&FlowMeterSerial={flowMeterSerial}";
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("ApiKey", "8172e4e0-d102-491b-a23b-4410cf0b24c6");
                var response = await client.GetAsync(fullUrl);
                Console.WriteLine($"API Response: Status Code - {response.StatusCode}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic? apiResponseObject = JsonConvert.DeserializeObject(content);
                    dynamic? profileReportModel = apiResponseObject.ProfileReportModel[0];
                    var profileReport = new ProfileReportModel
                    {
                        InstantFlow = profileReportModel.InstantFlow,
                        TotalConsumption = profileReportModel.TotalConsumption,
                        PersianDateTimeProfile = profileReportModel.PersianDateTimeProfile,
                        GreDateTimeProfile = profileReportModel.GreDateTimeProfile
                    };
                    db?.Pouyak.Add(profileReport);
                    db?.SaveChanges();
                    Console.WriteLine($"API Response Content: {content}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling API");
            }
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
