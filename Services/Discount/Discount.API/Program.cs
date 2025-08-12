using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Core.Repositories;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateDiscountHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddGrpc();

var app = builder.Build();

// Migrate the database
app.MigrateDataBase<Program>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<DiscountService>();
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Use a gRPC client to communicate with this service.");
    });
});

app.Run();
