using Ephemeral.App.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var backendUri = new Uri(builder.Configuration["BackendUri"]!);

builder.Services.AddScoped(sp =>
	new EphemeralApi(new()
	{
		BaseAddress = backendUri
	}));

await builder.Build().RunAsync();
