using Ephemeral.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

const string databaseName = "Secrets";
const int maxLifetime = (int)TimeSpan.SecondsPerDay;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString(databaseName) ?? $"Data Source={databaseName}.db";
builder.Services.AddSqlite<SecretDb>(connectionString);
builder.Services.AddHostedService<CleanupHostedService>();

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapPut("/secrets", SetSecret);
app.MapGet("/secrets/{id}", GetSecret);
app.Run();

[EndpointSummary("Conceal a secret")]
[EndpointDescription("Sets a secret with the specified ciphertext and time-to-live")]
[Produces<Guid>()]
static async Task<IResult> SetSecret(
	[FromQuery(Name = "ciphertext")] string ciphertext,
	[FromQuery(Name = "ttl")] int ttl,
	[FromServices] SecretDb db)
{
	ttl = Math.Max(0, Math.Min(ttl, maxLifetime));

	var secret = new Secret(ciphertext, TimeSpan.FromSeconds(ttl));
	var task = await db.Secrets.AddAsync(secret);
	await db.SaveChangesAsync();

	return Results.Ok(task.Entity.Id);
}

[EndpointSummary("Reveal a secret")]
[EndpointDescription("Gets a secret if it still exists and is unexpired")]
[Produces<string>()]
static async Task<IResult> GetSecret(
	[FromRoute] Guid id,
	[FromServices] SecretDb db)
{
	var secret = await db.Secrets.FindAsync(id);
	if (secret is null) return Results.NotFound();

	db.Secrets.Remove(secret);
	await db.SaveChangesAsync();

	return secret.Expiration < DateTime.UtcNow
		? Results.NotFound()
		: Results.Ok(secret.Ciphertext);
}