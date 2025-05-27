using Ephemeral.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

const string databaseName = "Secrets";
const int minLifetime = 0;
const int maxLifetime = 60 * 60 * 24;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SecretDb>(opt => opt.UseInMemoryDatabase(databaseName));

var app = builder.Build();
app.MapPut("/secrets", SetSecret);
app.MapGet("/secrets/{id}", GetSecret);

app.Run();

// Conceal secret with specified ciphertext and time-to-live.
static async Task<IResult> SetSecret(
	[FromQuery(Name = "ciphertext")] string ciphertext,
	[FromQuery(Name = "ttl")] int ttl,
	[FromServices] SecretDb db)
{
	ttl = Math.Max(minLifetime, Math.Min(ttl, maxLifetime));

	var secret = new Secret(ciphertext, TimeSpan.FromSeconds(ttl));
	var task = await db.Secrets.AddAsync(secret);
	await db.SaveChangesAsync();

	return Results.Ok(task.Entity.Id);
}

// Reveal secret if still unread and unexpired.
static async Task<IResult> GetSecret(
	[FromRoute] int id,
	[FromServices] SecretDb db)
{
	var secret = await db.Secrets.FindAsync(id);
	if (secret is null || secret.IsRead || secret.Expiration < DateTime.UtcNow)
		return Results.NotFound();

	secret.IsRead = true;
	await db.SaveChangesAsync();

	return Results.Ok(secret.Ciphertext);
}