using Microsoft.EntityFrameworkCore;
using OrderService.WebApi.Infrastructure;
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

// Регистрация PaymentsClient
builder.Services.AddScoped<PaymentsClient>();

// Регистрация Kafka
builder.Services.AddSingleton<KafkaProducer>();

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