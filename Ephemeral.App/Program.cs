using Ephemeral.App.Components;
using _Imports = Ephemeral.App.Client.Components._Imports;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services
			.AddRazorComponents()
			.AddInteractiveWebAssemblyComponents();

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/Error", true);
			app.UseHsts(); // https://aka.ms/aspnetcore-hsts
		}

		app.UseHttpsRedirection();
		app.UseAntiforgery();

		app.MapStaticAssets();
		app.MapRazorComponents<App>()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(_Imports).Assembly);

		app.Run();
	}
}