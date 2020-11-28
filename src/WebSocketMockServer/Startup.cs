using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WebSocketMockServer.Configuration;
using WebSocketMockServer.Loader;
using WebSocketMockServer.Middleware;
using WebSocketMockServer.Services;
using WebSocketMockServer.Storage;

namespace WebSocketMockServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddSingleton<IMockTemplateStorage, MockTemplateStorage>();
            services.Configure<FileLoaderConfiguration>(Configuration.GetSection(nameof(FileLoaderConfiguration)));
            services.AddSingleton<ILoader, FileLoader>();
            services.AddHostedService<LoaderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHealthChecks("/hc");

            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30)
            });

            app.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
