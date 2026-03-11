using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ExpressZen.Client;
using ExpressZen.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ExpressZen Services
builder.Services.AddScoped<IRegexEngine, DotNetRegexEngine>();
builder.Services.AddScoped<GeminiAssistantService>();
builder.Services.AddScoped<AnthropicAssistantService>();
builder.Services.AddScoped<OpenAiAssistantService>();
builder.Services.AddScoped<IAiAssistantService, AiServiceRouter>();

await builder.Build().RunAsync();
