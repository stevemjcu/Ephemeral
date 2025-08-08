using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ephemeral.Api.Services
{
	/// <summary>
	/// Represents a database session used to query and save secrets.
	/// </summary>
	public class SecretDatabase(DbContextOptions<SecretDatabase> options) : DbContext(options)
	{
		public DbSet<Secret> Secrets => Set<Secret>();
	}

	/// <summary>
	/// Represents a single-use and self-destructing secret.
	/// </summary>
	public class Secret(byte[] data, TimeSpan lifetime)
	{
		[Key] public Guid Id { get; init; }
		public DateTime Expiration { get; init; } = DateTime.UtcNow + lifetime;
		public byte[] Data { get; init; } = data;

		public Secret() : this([], default) { }
	}
}
