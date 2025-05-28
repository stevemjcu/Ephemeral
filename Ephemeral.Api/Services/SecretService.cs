using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Ephemeral.Api.Services
{
	/// <summary>
	/// Represents a database session used to query and save secrets.
	/// </summary>
	public class SecretService(DbContextOptions<SecretService> options) : DbContext(options)
	{
		public DbSet<Secret> Secrets => Set<Secret>();
	}

	/// <summary>
	/// Represents a single-use and self-destructing secret.
	/// </summary>
	public class Secret(string ciphertext, TimeSpan ttl)
	{
		[Key] public Guid Id { get; init; }
		public string Ciphertext { get; init; } = ciphertext;
		public DateTime Expiration { get; init; } = DateTime.UtcNow + ttl;

		public Secret() : this(string.Empty, default) { }
	}
}
