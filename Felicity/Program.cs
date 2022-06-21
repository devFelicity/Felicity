using Felicity.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDiscord(
        discordClient =>
        {
            // configure your client here
        },
        interactionService =>
        {
            // configure your interaction service here
        },
        textCommandsService =>
        {
            // configure your text commands service here
        },
        builder.Configuration);

var app = builder.Build();
await app.RunAsync();