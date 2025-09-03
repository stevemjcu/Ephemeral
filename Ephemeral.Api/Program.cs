using Ephemeral.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

internal class Program
{
	static readonly TimeSpan DefaultLifetime = TimeSpan.FromMinutes(5);
	static readonly TimeSpan MinLifetime = TimeSpan.FromSeconds(0);
	static readonly TimeSpan MaxLifetime = TimeSpan.FromDays(1);
	static readonly int MaxLength = 512;

	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		var connectionString = builder.Configuration.GetConnectionString("Secrets");
		var frontendUri = builder.Configuration["FrontendUri"]!;
		var cleanupInterval = TimeSpan.Parse(builder.Configuration["CleanupInterval"]!);

		builder.Services
			.AddCors(
				options => options.AddDefaultPolicy(
					policy => policy
						.WithOrigins(frontendUri)
						.AllowAnyMethod()
						.AllowAnyHeader()))
			.AddOpenApi()
			.AddSqlite<SecretDb>(connectionString)
			.AddHostedService(provider => new SecretCleanupJob(provider, cleanupInterval));

		var app = builder.Build();

		app.UseCors();

		if (app.Environment.IsDevelopment()) app.MapOpenApi();
		app.MapPost("/secrets", SetSecret);
		app.MapGet("/secrets/{id}", GetSecret);

		app.Run();
	}

	[EndpointSummary("Conceal a secret")]
	[EndpointDescription("Sets a secret with the specified ciphertext and time-to-live")]
	[Produces<Guid>()]
	static async Task<IResult> SetSecret(
		[FromQuery(Name = "ciphertext")] string ciphertext,
		[FromQuery(Name = "lifetime")] TimeSpan? lifetime,
		[FromServices] SecretDb database)
	{
		lifetime ??= DefaultLifetime;

		if (lifetime < MinLifetime || lifetime > MaxLifetime) return Results.BadRequest();
		if (ciphertext.Length > MaxLength) return Results.BadRequest();

		var secret = new Secret(ciphertext, (TimeSpan)lifetime);
		var entry = await database.Secrets.AddAsync(secret);
		await database.SaveChangesAsync();

		return Results.Ok(entry.Entity.Id);
	}

	[EndpointSummary("Reveal a secret")]
	[EndpointDescription("Gets a secret if it still exists and is unexpired")]
	[Produces<string>()]
	static async Task<IResult> GetSecret(
		[FromRoute] Guid id,
		[FromServices] SecretDb database)
	{
		var secret = await database.Secrets.FindAsync(id);
		if (secret is null) return Results.NotFound();

		database.Secrets.Remove(secret);
		await database.SaveChangesAsync();

		return secret.Expiration < DateTime.UtcNow
			? Results.NotFound()
			: Results.Ok(secret.Data);
	}
}