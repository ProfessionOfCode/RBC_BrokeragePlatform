using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RBC.BrokeragePlatform.WPF.Interfaces.Services;
using RBC.BrokeragePlatform.WPF.Services;
using RBC.BrokeragePlatform.WPF.ViewModel;
using Serilog;
using Serilog.Events;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace RBC.BrokeragePlatform.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
#if DEBUG
                ?? "Development";
#else
                ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? "Production";
#endif

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();

            var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
            var logFilePath = Path.Combine(logDirectory, "wpf-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure =>
            {
                configure.ClearProviders();
                configure.AddSerilog(Log.Logger, dispose: false);
            });

            services.AddSingleton(Configuration);

            services.AddHttpClient(nameof(IApiService), client =>
            {
                client.BaseAddress = new Uri(Configuration["Api:BaseUrl"]!);
            });

            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<ISignalRService, SignalRService>();
            services.AddSingleton<IBrokerageService, BrokerageService>();

            services.AddSingleton<MainViewModel>();
            services.AddTransient<AccountListViewModel>();
            services.AddTransient<SelectedAccountViewModel>();
            services.AddTransient<PlaceOrderViewModel>();
            services.AddSingleton<MainWindow>();
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value != null ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class InverseNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value == null ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
