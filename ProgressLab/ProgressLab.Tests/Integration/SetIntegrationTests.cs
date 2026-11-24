using System.Net;
using System.Net.Http.Json;
using Exercises;
using ExercisesMuscleGroups;
using MuscleGroups;
using Sets;
using Workouts;

namespace ProgressLab.Tests.Integration;

public class SetIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SetIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_WhenValidData_ReturnsCreatedSetThatCanBeRetrieved()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();

        var dto = new CreateSetDto
        {
            Repetitions = 10,
            Weight = 50m
        };

        // Act - Create set
        var response = await _client.PostAsJsonAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets",
            dto
        );

        // Assert - Creation response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdSet = await response.Content
            .ReadFromJsonAsync<SetDetailedDto>();

        Assert.NotNull(createdSet);
        Assert.NotEqual(Guid.Empty, createdSet.SetId);
        Assert.Equal(1, createdSet.SetNumber);
        Assert.Equal(dto.Repetitions, createdSet.Repetitions);
        Assert.Equal(dto.Weight, createdSet.Weight);

        // Act - Retrieve created set
        var getResponse = await _client.GetAsync(
            $"/api/sets/{createdSet.SetId}"
        );

        // Assert - Set was persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedSet = await getResponse.Content
            .ReadFromJsonAsync<SetDetailedDto>();

        Assert.NotNull(retrievedSet);
        Assert.Equal(createdSet.SetId, retrievedSet.SetId);
        Assert.Equal(1, retrievedSet.SetNumber);
        Assert.Equal(10, retrievedSet.Repetitions);
        Assert.Equal(50m, retrievedSet.Weight);
    }

    [Fact]
    public async Task Create_WhenWorkoutExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutExerciseId = Guid.NewGuid();

        var dto = new CreateSetDto
        {
            Repetitions = 10,
            Weight = 50m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenRepetitionsAreInvalid_ReturnsBadRequest()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();

        var dto = new CreateSetDto
        {
            Repetitions = 0,
            Weight = 50m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenSetExists_ReturnsSet()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();
        var set = await CreateSetAsync(workoutExerciseId, 12, 60m);

        // Act
        var response = await _client.GetAsync(
            $"/api/sets/{set.SetId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var retrievedSet = await response.Content
            .ReadFromJsonAsync<SetDetailedDto>();

        Assert.NotNull(retrievedSet);
        Assert.Equal(set.SetId, retrievedSet.SetId);
        Assert.Equal(set.SetNumber, retrievedSet.SetNumber);
        Assert.Equal(set.Repetitions, retrievedSet.Repetitions);
        Assert.Equal(set.Weight, retrievedSet.Weight);
    }

    [Fact]
    public async Task GetById_WhenSetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var setId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/sets/{setId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenSetsExist_ReturnsCreatedSets()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();
        var set = await CreateSetAsync(workoutExerciseId, 79, 75m);

        // Act
        var response = await _client.GetAsync(
            "/api/sets?page=1&pageSize=100"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sets = await response.Content
            .ReadFromJsonAsync<List<SetSummaryDto>>();

        Assert.NotNull(sets);

        Assert.Contains(
            sets,
            s => s.SetNumber == set.SetNumber &&
                 s.Repetitions == 79
        );
    }

    [Fact]
    public async Task GetAllByWorkoutExerciseId_WhenSetsExist_ReturnsOrderedSets()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();

        var firstSet = await CreateSetAsync(workoutExerciseId, 12, 40m);
        var secondSet = await CreateSetAsync(workoutExerciseId, 10, 45m);
        var thirdSet = await CreateSetAsync(workoutExerciseId, 8, 50m);

        // Act
        var response = await _client.GetAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sets = await response.Content
            .ReadFromJsonAsync<List<SetSummaryDto>>();

        Assert.NotNull(sets);
        Assert.Equal(3, sets.Count);

        Assert.Equal(
            new[] { 1, 2, 3 },
            sets.Select(s => s.SetNumber)
        );

        Assert.Equal(
            new[] { firstSet.Repetitions, secondSet.Repetitions, thirdSet.Repetitions },
            sets.Select(s => s.Repetitions)
        );
    }

    [Fact]
    public async Task GetAllByWorkoutExerciseId_WhenWorkoutExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutExerciseId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenSetExists_UpdatesAndPersistsValues()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();
        var set = await CreateSetAsync(workoutExerciseId, 10, 50m);

        var dto = new UpdateSetDto
        {
            Repetitions = 8,
            Weight = 60m
        };

        // Act - Update set
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/sets/{set.SetId}",
            dto
        );

        // Assert - Update response
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedSet = await updateResponse.Content
            .ReadFromJsonAsync<SetDetailedDto>();

        Assert.NotNull(updatedSet);
        Assert.Equal(set.SetId, updatedSet.SetId);
        Assert.Equal(set.SetNumber, updatedSet.SetNumber);
        Assert.Equal(8, updatedSet.Repetitions);
        Assert.Equal(60m, updatedSet.Weight);

        // Act - Retrieve updated set
        var getResponse = await _client.GetAsync(
            $"/api/sets/{set.SetId}"
        );

        // Assert - Updated values were persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedSet = await getResponse.Content
            .ReadFromJsonAsync<SetDetailedDto>();

        Assert.NotNull(retrievedSet);
        Assert.Equal(8, retrievedSet.Repetitions);
        Assert.Equal(60m, retrievedSet.Weight);
    }

    [Fact]
    public async Task Update_WhenSetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var dto = new UpdateSetDto
        {
            Repetitions = 8,
            Weight = 60m
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/sets/{setId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenRepetitionsAreInvalid_ReturnsBadRequest()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();
        var set = await CreateSetAsync(workoutExerciseId, 10, 50m);

        var dto = new UpdateSetDto
        {
            Repetitions = 0,
            Weight = 60m
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/sets/{set.SetId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenSetExists_DeletesAndReordersRemainingSets()
    {
        // Arrange
        var workoutExerciseId = await CreateWorkoutExerciseAsync();

        var firstSet = await CreateSetAsync(workoutExerciseId, 12, 40m);
        var setToDelete = await CreateSetAsync(workoutExerciseId, 10, 45m);
        var thirdSet = await CreateSetAsync(workoutExerciseId, 8, 50m);

        // Act - Delete second set
        var deleteResponse = await _client.DeleteAsync(
            $"/api/sets/{setToDelete.SetId}"
        );

        // Assert - Deletion response
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Act - Retrieve deleted set
        var deletedSetResponse = await _client.GetAsync(
            $"/api/sets/{setToDelete.SetId}"
        );

        // Assert - Set no longer exists
        Assert.Equal(HttpStatusCode.NotFound, deletedSetResponse.StatusCode);

        // Act - Retrieve remaining sets
        var getResponse = await _client.GetAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets"
        );

        // Assert - Remaining sets were correctly reordered
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var remainingSets = await getResponse.Content
            .ReadFromJsonAsync<List<SetSummaryDto>>();

        Assert.NotNull(remainingSets);
        Assert.Equal(2, remainingSets.Count);

        Assert.Equal(
            new[] { 1, 2 },
            remainingSets.Select(s => s.SetNumber)
        );

        Assert.Equal(
            new[] { firstSet.Repetitions, thirdSet.Repetitions },
            remainingSets.Select(s => s.Repetitions)
        );
    }

    [Fact]
    public async Task Delete_WhenSetDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var setId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/sets/{setId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<SetDetailedDto> CreateSetAsync(
        Guid workoutExerciseId,
        int repetitions,
        decimal weight)
    {
        var dto = new CreateSetDto
        {
            Repetitions = repetitions,
            Weight = weight
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/workout-exercises/{workoutExerciseId}/sets",
            dto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var set = await response.Content
            .ReadFromJsonAsync<SetDetailedDto>();

        Assert.NotNull(set);

        return set;
    }

    private async Task<Guid> CreateWorkoutExerciseAsync()
    {
        // Create muscle group
        var muscleGroup = new MuscleGroupDetailedDto();

        var muscleGroupDto = new CreateMuscleGroupDto
        {
            Name = $"Test Muscle Group {Guid.NewGuid():N}",
            Description = "Integration test muscle group"
        };

        var muscleGroupResponse = await _client.PostAsJsonAsync(
            "/api/muscle-groups",
            muscleGroupDto
        );

        Assert.Equal(HttpStatusCode.Created, muscleGroupResponse.StatusCode);

        var createdMuscleGroup = await muscleGroupResponse.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(createdMuscleGroup);

        muscleGroup = createdMuscleGroup;

        // Create exercise
        var exerciseDto = new CreateExerciseDto
        {
            Name = $"Test Exercise {Guid.NewGuid():N}",
            Description = "Integration test exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        var exerciseResponse = await _client.PostAsJsonAsync(
            "/api/exercises",
            exerciseDto
        );

        Assert.Equal(HttpStatusCode.Created, exerciseResponse.StatusCode);

        var exercise = await exerciseResponse.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(exercise);

        // Create workout
        var workoutDto = new CreateWorkoutDto
        {
            Name = $"Test Workout {Guid.NewGuid():N}",
            Date = DateTimeOffset.UtcNow,
            Notes = "Integration test workout"
        };

        var workoutResponse = await _client.PostAsJsonAsync(
            "/api/workouts",
            workoutDto
        );

        Assert.Equal(HttpStatusCode.Created, workoutResponse.StatusCode);

        var workout = await workoutResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(workout);

        // Associate exercise with workout
        var addExerciseResponse = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        Assert.Equal(HttpStatusCode.NoContent, addExerciseResponse.StatusCode);

        // Retrieve WorkoutExercise ID
        var getWorkoutResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        Assert.Equal(HttpStatusCode.OK, getWorkoutResponse.StatusCode);

        var updatedWorkout = await getWorkoutResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(updatedWorkout);

        var workoutExercise = Assert.Single(updatedWorkout.WorkoutExercises);

        return workoutExercise.WorkoutExerciseId;
    }
}
