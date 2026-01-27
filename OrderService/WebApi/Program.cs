using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.DataAccess.Postgres.AppDbContext;
using OrderService.WebApi.Infrastructure;
using OrderService.WebApi.Mappers;
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
builder.Services.AddScoped<IPaymentsClient, PaymentsClient>();

// Регистрация Kafka
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

// Регистрации Mapperly
builder.Services.AddSingleton<OrderMapper>();

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly); // Регистрируем все валидаторы

// Регистрация MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>(); // Добавляем фильтр здесь
});

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