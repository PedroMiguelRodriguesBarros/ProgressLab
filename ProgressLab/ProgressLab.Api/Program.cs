using Data.Context;
using Microsoft.EntityFrameworkCore;
using MuscleGroups;

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

// Repositories
// Registar aqui o MuscleGroupRepository quando estiver implementado.

// Services
builder.Services.AddScoped<MuscleGroupService>();

// Mappers
builder.Services.AddScoped<
    IMapper<MuscleGroup, MuscleGroupDetailedDto>,
    MuscleGroupEntityToDetailedDtoMapper>();

builder.Services.AddScoped<
    IMapper<MuscleGroup, MuscleGroupSummaryDto>,
    MuscleGroupEntityToSummaryDtoMapper>();

builder.Services.AddScoped<
    IMapper<CreateMuscleGroupDto, MuscleGroup>,
    CreateMuscleGroupDtoToEntityMapper>();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();