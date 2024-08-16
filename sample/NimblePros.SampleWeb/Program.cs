using System.Reflection;
using FastEndpoints;
using FluentValidation;
using NimblePros.MediatR.Contrib.Behaviors;
using NimblePros.SampleWeb;
using NimblePros.SampleWeb.Commands;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console()
  .CreateLogger();

logger.Information("Starting web host");


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddHttpLogging(o => { });

builder.Services.AddFastEndpoints();

// Set up MediatR
builder.Services.AddMediatR(cfg =>
{
  cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
  cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
  cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
  cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
});
//builder.Services.AddMediatRLoggingBehavior();
//builder.Services.AddMediatRFluentValidationBehavior();
//builder.Services.AddValidatorsFromAssemblyContaining<AddItemToCartCommandValidator>();
//builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>(); // domain events

builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();
builder.Services.AddMemoryCache();

var constr = builder.Configuration.GetConnectionString("Constr");
builder.Services.AddSqlServer<CustomerContext>(constr);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpLogging();

app.UseFastEndpoints();

app.Run();

