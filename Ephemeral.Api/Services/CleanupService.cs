using Microsoft.EntityFrameworkCore;

namespace Ephemeral.Api.Services
{
	/// <summary>
	/// Represents an ongoing background job to clean up expired secrets.
	/// </summary>
	public class CleanupService(IServiceProvider services, TimeSpan interval) : BackgroundService
	{
		private readonly IServiceProvider _services = services;

		protected async override Task ExecuteAsync(CancellationToken ct)
		{
			using var timer = new PeriodicTimer(interval);

			while (await timer.WaitForNextTickAsync(ct))
			{
				using var scope = _services.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<SecretService>();
				await db.Secrets.Where(s => s.Expiration < DateTime.UtcNow).ExecuteDeleteAsync(ct);
			}
		}
	}
}
