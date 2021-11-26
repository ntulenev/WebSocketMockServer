using WebSocketMockServer.Configuration;
using WebSocketMockServer.Loader;
using WebSocketMockServer.Middleware;
using WebSocketMockServer.Services;
using WebSocketMockServer.Storage;
using WebSocketMockServer.WebSockets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IMockTemplateStorage, MockTemplateStorage>();
builder.Services.AddSingleton<IWebSocketHandler, WebSocketHandler>();
builder.Services.Configure<FileLoaderConfiguration>(builder.Configuration.GetSection(nameof(FileLoaderConfiguration)));
builder.Services.AddSingleton<ILoader, FileLoader>();
builder.Services.AddHostedService<LoaderService>();

using var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseHealthChecks("/hc");
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30)
});
app.UseMiddleware<CustomWebSocketMiddleware>();

app.Run();

#pragma warning disable CA1050 // Declare types in namespaces
// Make the implicit Program class public so test projects can access it
public partial class Program { }
#pragma warning restore CA1050 // Declare types in namespaces

