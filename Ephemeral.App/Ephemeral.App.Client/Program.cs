using Ephemeral.App.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);

		var backendUri = new Uri(builder.Configuration["BackendUri"]!);

		builder.Services.AddScoped(sp =>
			new SecretsApi(new()
			{
				BaseAddress = backendUri
			}));

		await builder.Build().RunAsync();
	}
}