using Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

// Database connection
var connectionString = builder.Configuration
    .GetConnectionString("ProgressLabConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'ProgressLabConnection' not found."
    );

builder.Services.AddDbContext<ProgressLabDbContext>(options =>
    options.UseNpgsql(connectionString)
);

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();