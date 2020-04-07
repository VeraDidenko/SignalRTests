using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SignalRTests
{
    public class Program
    {
        private static readonly Dictionary<string, string> DEFAULTS = new Dictionary<string, string>
            {
                { "transport", "websockets"},
                { "port", "5000"}
            };
        private static readonly Dictionary<string, string> ARGS_MAPPING = new Dictionary<string, string>
            {
                { "-t", "transport" },
                { "-port", "port" }
            };
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            System.Threading.Thread.Sleep(-1);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(DEFAULTS)
                .AddCommandLine(args, Program.ARGS_MAPPING);
            var configuration = configBuilder.Build();

            var port = configuration.GetValue<string>("port");

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    configBuilder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseUrls("http://0.0.0.0:" + port);
                });
            return hostBuilder;
        }
    }
}
