using AdventOfCode;
using AdventOfCode.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsetting.Local.json");
builder.Logging.AddSerilog().AddConsole();

builder.Services.AddOptions<InputSettings>()
    .Bind(builder.Configuration.GetSection(InputSettings.SectionKey));
builder.Services.AddOptions<DateSettings>()
    .Bind(builder.Configuration.GetSection(DateSettings.SectionKey));

builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IInputProvider, InputClient>();
builder.Services.AddScoped<IPuzzleFactory, PuzzleFactory>();

builder.Services.AddHostedService<PuzzleHost>();

var app = builder.Build();

app.Run();