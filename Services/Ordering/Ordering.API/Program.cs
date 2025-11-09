using Asp.Versioning;
using Common.Logging;
using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumer;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Extensions;
using Serilog;
using static MassTransit.Logging.LogCategoryName;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Serilog configuration
builder.Host.UseSerilog(Logging.ConfigureLogger);

builder.Services.AddControllers();

// API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// API Explorer (for Swagger)
builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Ordering.API", Version = "v1" });
});

// Application Services
builder.Services.AddApplicationServices();
// Infrastructure Services
builder.Services.AddInfraServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Consumer class
builder.Services.AddScoped<BasketOrderingConsumer>();
builder.Services.AddScoped<BasketOrderingConsumerV2>();

// MassTransit configuration
builder.Services.AddMassTransit(config =>
{
    // Đăng ký một consumer tên là BasketOrderingConsumer 
    config.AddConsumer<BasketOrderingConsumer>();   // consumer xử lý message kiểu BasketOrdering
    config.AddConsumer<BasketOrderingConsumerV2>();   // consumer xử lý message kiểu BasketOrdering

    //Chỉ định transport là RabbitMQ.
    config.UsingRabbitMq((context, cfg) =>
    {
        // Kết nối tới RabbitMQ host đang được cấu hình trong appsettings.json
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        // đặt tên cho queue, sau đó chỉ định consumer nào sẽ xử lý message từ queue này
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueue, e =>
        {
            e.ConfigureConsumer<BasketOrderingConsumer>(context);
        });

        // V2 Version
        cfg.ReceiveEndpoint(EventBusConstant.BasketCheckoutQueueV2, e =>
        {
            e.ConfigureConsumer<BasketOrderingConsumerV2>(context);
        });
    });

    

});

var app = builder.Build();

// Apply database migrations
app.MigrationDatabase<OrderContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderContextSeed>>();
    if (context == null)
    {
        logger?.LogError("OrderContext is null during migration.");
        return;
    }
    OrderContextSeed.SeedAsync(context, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
