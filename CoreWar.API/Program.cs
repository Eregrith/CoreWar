using Corewar.Core.Parser;
using Corewar.Core.Random;
using CoreWar.API.Commands.CreateCoreWarBattle;
using CoreWar.API.Commands.Step;
using CoreWar.API.CorewarBattle;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(Program));

builder.Services.AddTransient<IRandomGenerator, RandomGenerator>();
builder.Services.AddSingleton<ICoreWarBattleManager, CoreWarBattleManager>();
builder.Services.AddSingleton<IRedcodeToBinary, RedcodeToBinary>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/CreateBattle", async (ILoggerFactory loggerFactory, IMediator mediator, CreateCoreWarBattleCommand command, CancellationToken token) =>
{
    var logger = loggerFactory.CreateLogger("CreateBattle");
    logger.LogInformation("Creating battle with command {command}", command);
    var result = await mediator.Send(command, token);
    return result;
});

app.MapPost("/Step", async (ILoggerFactory loggerFactory, IMediator mediator, CancellationToken token) =>
{
    var logger = loggerFactory.CreateLogger("Step");
    logger.LogInformation("Stepping forward");
    var result = await mediator.Send(new StepCommand(), token);
    return result;
});

app.Run();