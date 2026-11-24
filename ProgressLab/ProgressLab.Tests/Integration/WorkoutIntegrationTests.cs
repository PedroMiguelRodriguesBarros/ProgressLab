using System.Net;
using System.Net.Http.Json;
using Exercises;
using ExercisesMuscleGroups;
using MuscleGroups;
using Sets;
using Workouts;

namespace ProgressLab.Tests.Integration;

public class WorkoutIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WorkoutIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_WhenValidData_ReturnsCreatedWorkoutThatCanBeRetrieved()
    {
        // Arrange
        var dto = new CreateWorkoutDto
        {
            Name = $"Upper Body {Guid.NewGuid():N}",
            Date = DateTimeOffset.UtcNow,
            Notes = "Training session"
        };

        // Act - Create workout
        var response = await _client.PostAsJsonAsync("/api/workouts", dto);

        // Assert - Creation response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdWorkout = await response.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(createdWorkout);
        Assert.NotEqual(Guid.Empty, createdWorkout.WorkoutId);
        Assert.Equal(dto.Name, createdWorkout.Name);
        Assert.Equal(dto.Date, createdWorkout.Date);
        Assert.Equal(dto.Notes, createdWorkout.Notes);

        // Act - Retrieve created workout
        var getResponse = await _client.GetAsync(
            $"/api/workouts/{createdWorkout.WorkoutId}"
        );

        // Assert - Workout was persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedWorkout = await getResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(retrievedWorkout);
        Assert.Equal(createdWorkout.WorkoutId, retrievedWorkout.WorkoutId);
        Assert.Equal(dto.Name, retrievedWorkout.Name);
    }

    [Fact]
    public async Task Create_WhenNameIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateWorkoutDto
        {
            Name = "",
            Date = DateTimeOffset.UtcNow,
            Notes = null
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/workouts", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenWorkoutExists_ReturnsWorkout()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var retrievedWorkout = await response.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(retrievedWorkout);
        Assert.Equal(workout.WorkoutId, retrievedWorkout.WorkoutId);
        Assert.Equal(workout.Name, retrievedWorkout.Name);
    }

    [Fact]
    public async Task GetById_WhenWorkoutDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/workouts/{workoutId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenWorkoutsExist_ReturnsCreatedWorkouts()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        // Act
        var response = await _client.GetAsync(
            "/api/workouts?page=1&pageSize=100"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var workouts = await response.Content
            .ReadFromJsonAsync<List<WorkoutSummaryDto>>();

        Assert.NotNull(workouts);
        Assert.Contains(workouts, w => w.WorkoutId == workout.WorkoutId);
    }

    [Fact]
    public async Task Update_WhenWorkoutExists_UpdatesAndPersistsValues()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        var updatedDate = DateTimeOffset.UtcNow.AddDays(-1);

        var dto = new UpdateWorkoutDto
        {
            Name = "  Updated Upper Body  ",
            Date = updatedDate,
            Notes = "  Updated notes  "
        };

        // Act - Update workout
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/workouts/{workout.WorkoutId}",
            dto
        );

        // Assert - Update response
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedWorkout = await updateResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(updatedWorkout);
        Assert.Equal("Updated Upper Body", updatedWorkout.Name);
        AssertDateTimeOffsetEqualToPostgresPrecision(updatedDate, updatedWorkout.Date);
        Assert.Equal("Updated notes", updatedWorkout.Notes);

        // Act - Retrieve updated workout
        var getResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        // Assert - Updated values were persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedWorkout = await getResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(retrievedWorkout);
        Assert.Equal("Updated Upper Body", retrievedWorkout.Name);
        AssertDateTimeOffsetEqualToPostgresPrecision(updatedDate, retrievedWorkout.Date);
        Assert.Equal("Updated notes", retrievedWorkout.Notes);
    }

    [Fact]
    public async Task Update_WhenWorkoutDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();

        var dto = new UpdateWorkoutDto
        {
            Name = "Updated Workout",
            Date = DateTimeOffset.UtcNow,
            Notes = "Updated notes"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/workouts/{workoutId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenDateIsNull_PreservesDateAndClearsNotes()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        var dto = new UpdateWorkoutDto
        {
            Name = "Updated Workout",
            Date = null,
            Notes = null
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/workouts/{workout.WorkoutId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedWorkout = await response.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(updatedWorkout);
        AssertDateTimeOffsetEqualToPostgresPrecision(workout.Date, updatedWorkout.Date);
        Assert.Null(updatedWorkout.Notes);
    }

    [Fact]
    public async Task Delete_WhenWorkoutExists_ReturnsNoContentAndRemovesWorkout()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        // Act - Delete workout
        var deleteResponse = await _client.DeleteAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        // Assert - Deletion response
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Act - Retrieve deleted workout
        var getResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenWorkoutDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/workouts/{workoutId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenWorkoutHasExercisesAndSets_CascadesDeletion()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exercise = await CreateExerciseAsync();

        var addExerciseResponse = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        Assert.Equal(HttpStatusCode.NoContent, addExerciseResponse.StatusCode);

        var workoutResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        var updatedWorkout = await workoutResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(updatedWorkout);

        var workoutExercise = Assert.Single(updatedWorkout.WorkoutExercises);

        var setDto = new CreateSetDto
        {
            Repetitions = 10,
            Weight = 50m
        };

        var setResponse = await _client.PostAsJsonAsync(
            $"/api/workout-exercises/{workoutExercise.WorkoutExerciseId}/sets",
            setDto
        );

        Assert.Equal(HttpStatusCode.Created, setResponse.StatusCode);

        // Act - Delete workout containing an exercise and a set
        var deleteResponse = await _client.DeleteAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        // Assert - Workout and dependent records can be deleted
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var deletedWorkoutResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        Assert.Equal(HttpStatusCode.NotFound, deletedWorkoutResponse.StatusCode);

        // The exercise itself must still exist
        var exerciseResponse = await _client.GetAsync(
            $"/api/exercises/{exercise.ExerciseId}"
        );

        Assert.Equal(HttpStatusCode.OK, exerciseResponse.StatusCode);
    }

    [Fact]
    public async Task AddExercise_WhenValidData_ReturnsNoContentAndPersistsExercise()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exercise = await CreateExerciseAsync();

        // Act
        var response = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        var updatedWorkout = await getResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(updatedWorkout);

        var addedExercise = Assert.Single(updatedWorkout.WorkoutExercises);

        Assert.Equal(exercise.ExerciseId, addedExercise.ExerciseId);
        Assert.Equal(exercise.Name, addedExercise.ExerciseName);
        Assert.Equal(1, addedExercise.Order);
    }

    [Fact]
    public async Task AddExercise_WhenWorkoutDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exercise = await CreateExerciseAsync();

        // Act
        var response = await _client.PostAsync(
            $"/api/workouts/{workoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddExercise_WhenExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exerciseId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exerciseId}",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddExercise_WhenExerciseAlreadyBelongsToWorkout_ReturnsConflict()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exercise = await CreateExerciseAsync();

        var firstResponse = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        Assert.Equal(HttpStatusCode.NoContent, firstResponse.StatusCode);

        // Act - Attempt to add the same exercise again
        var secondResponse = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task RemoveExercise_WhenExerciseExists_RemovesAndReordersRemainingExercises()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        var firstExercise = await CreateExerciseAsync();
        var exerciseToRemove = await CreateExerciseAsync();
        var thirdExercise = await CreateExerciseAsync();

        var exerciseIds = new[]
        {
            firstExercise.ExerciseId,
            exerciseToRemove.ExerciseId,
            thirdExercise.ExerciseId
        };

        foreach (var exerciseId in exerciseIds)
        {
            var addResponse = await _client.PostAsync(
                $"/api/workouts/{workout.WorkoutId}/exercises/{exerciseId}",
                null
            );

            Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);
        }

        // Act - Remove the second exercise
        var removeResponse = await _client.DeleteAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exerciseToRemove.ExerciseId}"
        );

        // Assert - Removal response
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        // Act - Retrieve updated workout
        var getResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        var updatedWorkout = await getResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        // Assert - Remaining exercises are correctly reordered
        Assert.NotNull(updatedWorkout);
        Assert.Equal(2, updatedWorkout.WorkoutExercises.Count);

        var remainingExercises = updatedWorkout.WorkoutExercises
            .OrderBy(we => we.Order)
            .ToList();

        Assert.Equal(
            new[] { firstExercise.ExerciseId, thirdExercise.ExerciseId },
            remainingExercises.Select(we => we.ExerciseId)
        );

        Assert.Equal(
            new[] { 1, 2 },
            remainingExercises.Select(we => we.Order)
        );
    }

    [Fact]
    public async Task RemoveExercise_WhenWorkoutDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exercise = await CreateExerciseAsync();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/workouts/{workoutId}/exercises/{exercise.ExerciseId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveExercise_WhenExerciseIsNotPartOfWorkout_ReturnsNotFound()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exercise = await CreateExerciseAsync();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateExerciseOrder_WhenNewOrderIsValid_ReordersAndPersistsExercises()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();

        var firstExercise = await CreateExerciseAsync();
        var secondExercise = await CreateExerciseAsync();
        var thirdExercise = await CreateExerciseAsync();

        var exerciseIds = new[]
        {
            firstExercise.ExerciseId,
            secondExercise.ExerciseId,
            thirdExercise.ExerciseId
        };

        foreach (var exerciseId in exerciseIds)
        {
            var addResponse = await _client.PostAsync(
                $"/api/workouts/{workout.WorkoutId}/exercises/{exerciseId}",
                null
            );

            Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);
        }

        var dto = new UpdateWorkoutExerciseOrderDto
        {
            NewOrder = 1
        };

        // Act - Move the third exercise to the first position
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{thirdExercise.ExerciseId}/order",
            dto
        );

        // Assert - Reordering response
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        // Act - Retrieve updated workout
        var getResponse = await _client.GetAsync(
            $"/api/workouts/{workout.WorkoutId}"
        );

        var updatedWorkout = await getResponse.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        // Assert - New order was persisted
        Assert.NotNull(updatedWorkout);

        var reorderedExercises = updatedWorkout.WorkoutExercises
            .OrderBy(we => we.Order)
            .ToList();

        Assert.Equal(
            new[]
            {
                thirdExercise.ExerciseId,
                firstExercise.ExerciseId,
                secondExercise.ExerciseId
            },
            reorderedExercises.Select(we => we.ExerciseId)
        );

        Assert.Equal(
            new[] { 1, 2, 3 },
            reorderedExercises.Select(we => we.Order)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task UpdateExerciseOrder_WhenNewOrderIsInvalid_ReturnsBadRequest(int newOrder)
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exercise = await CreateExerciseAsync();

        var addResponse = await _client.PostAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}",
            null
        );

        Assert.Equal(HttpStatusCode.NoContent, addResponse.StatusCode);

        var dto = new UpdateWorkoutExerciseOrderDto
        {
            NewOrder = newOrder
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}/order",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateExerciseOrder_WhenWorkoutDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exercise = await CreateExerciseAsync();

        var dto = new UpdateWorkoutExerciseOrderDto
        {
            NewOrder = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/workouts/{workoutId}/exercises/{exercise.ExerciseId}/order",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateExerciseOrder_WhenExerciseIsNotPartOfWorkout_ReturnsNotFound()
    {
        // Arrange
        var workout = await CreateWorkoutAsync();
        var exercise = await CreateExerciseAsync();

        var dto = new UpdateWorkoutExerciseOrderDto
        {
            NewOrder = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/workouts/{workout.WorkoutId}/exercises/{exercise.ExerciseId}/order",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<WorkoutDetailedDto> CreateWorkoutAsync()
    {
        var dto = new CreateWorkoutDto
        {
            Name = $"Test Workout {Guid.NewGuid():N}",
            Date = DateTimeOffset.UtcNow,
            Notes = "Integration test workout"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/workouts",
            dto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var workout = await response.Content
            .ReadFromJsonAsync<WorkoutDetailedDto>();

        Assert.NotNull(workout);

        return workout;
    }

    private async Task<MuscleGroupDetailedDto> CreateMuscleGroupAsync()
    {
        var dto = new CreateMuscleGroupDto
        {
            Name = $"Test Muscle Group {Guid.NewGuid():N}",
            Description = "Integration test muscle group"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/muscle-groups",
            dto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var muscleGroup = await response.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(muscleGroup);

        return muscleGroup;
    }

    private async Task<ExerciseDetailedDto> CreateExerciseAsync()
    {
        var muscleGroup = await CreateMuscleGroupAsync();

        var dto = new CreateExerciseDto
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

        var response = await _client.PostAsJsonAsync(
            "/api/exercises",
            dto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var exercise = await response.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(exercise);

        return exercise;
    }

    private static void AssertDateTimeOffsetEqualToPostgresPrecision(DateTimeOffset expected, DateTimeOffset actual)
    {
        var expectedTicks = expected.UtcDateTime.Ticks / 10;
        var actualTicks = actual.UtcDateTime.Ticks / 10;

        Assert.Equal(expectedTicks, actualTicks);
    }
}
