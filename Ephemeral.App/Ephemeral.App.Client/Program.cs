using Ephemeral.App.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var backendUri = builder.Configuration["BackendUri"] ?? "https://localhost:7133";

builder.Services.AddScoped(sp =>
	new EphemeralService(new()
	{
		BaseAddress = new Uri(backendUri)
	}));

await builder.Build().RunAsync();
