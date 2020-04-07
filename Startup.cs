using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRTests.Hubs;
using Microsoft.AspNetCore.Http.Connections;

namespace SignalRTests
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder
                        //.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowed((host) => true) //for signalr cors                
                    );
                }
            );
            services.AddSignalR(
                hubOptions =>
                    {
                        hubOptions.EnableDetailedErrors = true;
                    }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");

            string transportType = this.Configuration.GetValue<string>("transport");
            Console.WriteLine("TransportType: [ "  + transportType + " ]");
            HttpTransportType transportOptions = HttpTransportType.WebSockets;
            switch (transportType.ToLower())
            {
                case "websockets":
                case "ws":
                    transportOptions = HttpTransportType.WebSockets;
                    break;
                case "longpolling":
                case "lp":
                    transportOptions = HttpTransportType.LongPolling;
                    break;
                case "serversentevents":
                case "sse":
                    transportOptions = HttpTransportType.ServerSentEvents;
                    break;
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TestsHub>("/tests", options =>
                {
                    options.Transports = transportOptions;
                });
            });
        }
    }
}
