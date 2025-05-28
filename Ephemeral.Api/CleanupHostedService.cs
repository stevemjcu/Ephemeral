using Microsoft.EntityFrameworkCore;

namespace Ephemeral.Api
{
	public class CleanupHostedService(IServiceProvider services) : BackgroundService, IDisposable
	{
		private readonly IServiceProvider _services = services;
		private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));
		private bool _disposed;

		protected async override Task ExecuteAsync(CancellationToken ct)
		{
			while (await _timer.WaitForNextTickAsync(ct))
			{
				using var scope = _services.CreateScope();
				var service = scope.ServiceProvider.GetRequiredService<SecretDb>();
				await service.Secrets.Where(s => s.Expiration < DateTime.UtcNow).ExecuteDeleteAsync(ct);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// Dispose managed resources
					_timer.Dispose();
				}

				// Free unmanaged resources
				// Null large fields
				_disposed = true;
			}
		}

		public override void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			base.Dispose();
		}
	}
}
