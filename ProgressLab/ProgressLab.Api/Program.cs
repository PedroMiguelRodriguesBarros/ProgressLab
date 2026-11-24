using Data.Context;
using Microsoft.EntityFrameworkCore;
using MuscleGroups;
using Exercises;
using Workouts;
using Sets;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI
builder.Services.AddOpenApi();

//Controllers
builder.Services.AddControllers();

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
builder.Services.AddScoped<IMuscleGroupRepository, MuscleGroupRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<ISetRepository, SetRepository>();

// Services
builder.Services.AddScoped<MuscleGroupService>();
builder.Services.AddScoped<ExerciseService>();
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddScoped<SetService>();

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

builder.Services.AddScoped<
    IMapper<CreateExerciseDto, Exercise>,
    CreateExerciseDtoToEntityMapper>();

builder.Services.AddScoped<
    IMapper<Exercise, ExerciseDetailedDto>,
    ExerciseEntityToDetailedDtoMapper>();

builder.Services.AddScoped<
    IMapper<Exercise, ExerciseSummaryDto>,
    ExerciseEntityToSummaryDtoMapper>();

builder.Services.AddScoped<
    IMapper<CreateWorkoutDto, Workout>,
    CreateWorkoutDtoToEntityMapper>();

builder.Services.AddScoped<
    IMapper<Workout, WorkoutDetailedDto>,
    WorkoutEntityToDetailedDtoMapper>();

builder.Services.AddScoped<
    IMapper<Workout, WorkoutSummaryDto>,
    WorkoutEntityToSummaryDtoMapper>();

builder.Services.AddScoped<
    IMapper<Set, SetDetailedDto>,
    SetEntityToDetailedDtoMapper>();

builder.Services.AddScoped<
    IMapper<Set, SetSummaryDto>,
    SetEntityToSummaryDtoMapper>();

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
public partial class Program { }