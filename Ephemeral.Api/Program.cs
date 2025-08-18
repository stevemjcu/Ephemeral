using Ephemeral.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

const int minLifetime = 0;
const int maxLifetime = (int)TimeSpan.SecondsPerDay;

var builder = WebApplication.CreateBuilder(args);

var frontendUri = builder.Configuration["FrontendUri"]!;
var cleanupInterval = TimeSpan.FromSeconds(int.Parse(builder.Configuration["CleanupIntervalSeconds"]!));

builder.Services.AddCors(
	options => options.AddDefaultPolicy(
		policy => policy
			.WithOrigins(frontendUri)
			.AllowAnyMethod()
			.AllowAnyHeader()));

builder.Services.AddOpenApi();
builder.Services.AddDbContext<SecretDb>(options => options.UseInMemoryDatabase("Secrets"));
builder.Services.AddHostedService(provider => new SecretCleanupJob(provider, cleanupInterval));

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapPost("/secrets", SetSecret);
app.MapGet("/secrets/{id}", GetSecret);

app.Run();

[EndpointSummary("Conceal a secret")]
[EndpointDescription("Sets a secret with the specified ciphertext and time-to-live")]
[Produces<Guid>()]
static async Task<IResult> SetSecret(
	[FromQuery(Name = "ciphertext")] string ciphertext,
	[FromQuery(Name = "lifetime")] int lifetime,
	[FromServices] SecretDb database)
{
	lifetime = Math.Max(minLifetime, Math.Min(lifetime, maxLifetime));

	var secret = new Secret(ciphertext, TimeSpan.FromSeconds(lifetime));
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