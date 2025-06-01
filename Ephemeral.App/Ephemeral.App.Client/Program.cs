using Ephemeral.App.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp =>
	new EphemeralService(new HttpClient
	{
		BaseAddress = new Uri(builder.Configuration["EphemeralServiceUri"]!)
	}));

await builder.Build().RunAsync();
