using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace EventManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dateTimeNowString = $"{DateTime.Now:yyyy-MM-dd}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .WriteTo.Logger(
                    _ => _.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                        .WriteTo.File($"Logs/{dateTimeNowString}-Error.log",
                            rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 100000)
                )
                .WriteTo.Logger(
                    _ => _.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning)
                        .WriteTo.File($"Logs/{dateTimeNowString}-Warning.log",
                            rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 100000)
                )
                .WriteTo.File($"Logs/{dateTimeNowString}-All.log")
                .WriteTo.Console()
                .WriteTo.Seq("http://emseq:5341")
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An error occured while starting the application");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}