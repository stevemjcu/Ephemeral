using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ephemeral.Api.Services
{
	/// <summary>
	/// Represents a database connection used to query and save secrets.
	/// </summary>
	public class SecretDb(DbContextOptions<SecretDb> options) : DbContext(options)
	{
		public DbSet<Secret> Secrets => Set<Secret>();
	}

	/// <summary>
	/// Represents an expiring secret.
	/// </summary>
	public class Secret(string data, TimeSpan lifetime)
	{
		[Key] public Guid Id { get; init; }

		public DateTime Expiration { get; init; } = DateTime.UtcNow + lifetime;

		public string Data { get; init; } = data;

		public Secret() : this(string.Empty, default) { }
	}
}
