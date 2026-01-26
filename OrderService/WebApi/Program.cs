using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.WebApi.Infrastructure;
using OrderService.WebApi.Mappers;
using OrderService.WebApi.Validators;
using Refit;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core Postgres
builder.Services.AddDbContext<OrdersDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("OrdersDb");
    opt.UseNpgsql(cs);
});

// Регистрация PaymentsApi через Refit
builder.Services.AddTransient<IPaymentsApi>(sp =>
    RestService.For<IPaymentsApi>("http://payment-service:8080"));

// Регистрация MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Регистрация PaymentsClient
builder.Services.AddScoped<PaymentsClient>();

// Регистрация Kafka
builder.Services.AddSingleton<KafkaProducer>();

// Регистрации Mapperly
builder.Services.AddSingleton<OrderMapper>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();


// Настройка логирования
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddConsole();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok("order-service ok"));

app.Run();