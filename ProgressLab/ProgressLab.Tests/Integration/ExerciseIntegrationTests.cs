using System.Net;
using System.Net.Http.Json;
using Exercises;
using ExercisesMuscleGroups;
using MuscleGroups;

namespace ProgressLab.Tests.Integration;

public class ExerciseIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ExerciseIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_WhenValidData_ReturnsCreatedExerciseWithMuscleGroups()
    {
        // Arrange
        var primaryGroup = await CreateMuscleGroupAsync();
        var secondaryGroup = await CreateMuscleGroupAsync();

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = primaryGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = secondaryGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Secondary
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/exercises", dto);

        // Assert - Creation response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var createdExercise = await response.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(createdExercise);
        Assert.NotEqual(Guid.Empty, createdExercise.ExerciseId);
        Assert.Equal(dto.Name, createdExercise.Name);
        Assert.Equal(dto.Description, createdExercise.Description);

        Assert.Equal(2, createdExercise.MuscleGroups.Count);

        Assert.Contains(
            createdExercise.MuscleGroups,
            mg => mg.MuscleGroupId == primaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Primary &&
                  mg.MuscleGroupName == primaryGroup.Name
        );

        Assert.Contains(
            createdExercise.MuscleGroups,
            mg => mg.MuscleGroupId == secondaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Secondary &&
                  mg.MuscleGroupName == secondaryGroup.Name
        );

        // Act - Retrieve created exercise
        var getResponse = await _client.GetAsync(
            $"/api/exercises/{createdExercise.ExerciseId}"
        );

        // Assert - Relations were persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedExercise = await getResponse.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(retrievedExercise);
        Assert.Equal(createdExercise.ExerciseId, retrievedExercise.ExerciseId);
        Assert.Equal(2, retrievedExercise.MuscleGroups.Count);
        Assert.Contains(
            retrievedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == primaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Primary
        );
    }

    [Fact]
    public async Task Create_WhenNoPrimaryMuscleGroupIsProvided_ReturnsBadRequest()
    {
        // Arrange
        var secondaryGroup = await CreateMuscleGroupAsync();

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = secondaryGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Secondary
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/exercises", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenMultiplePrimaryMuscleGroupsAreProvided_ReturnsBadRequest()
    {
        // Arrange
        var firstGroup = await CreateMuscleGroupAsync();
        var secondGroup = await CreateMuscleGroupAsync();

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = firstGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = secondGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/exercises", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenMuscleGroupsAreDuplicated_ReturnsBadRequest()
    {
        // Arrange
        var muscleGroup = await CreateMuscleGroupAsync();

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Secondary
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/exercises", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenMuscleGroupDoesNotExist_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = Guid.NewGuid(),
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/exercises", dto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenExerciseExists_ReturnsExerciseWithMuscleGroups()
    {
        // Arrange
        var primaryGroup = await CreateMuscleGroupAsync();
        var secondaryGroup = await CreateMuscleGroupAsync();

        var exercise = await CreateExerciseAsync(
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = primaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Primary
            },
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = secondaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Secondary
            }
        );

        // Act
        var response = await _client.GetAsync(
            $"/api/exercises/{exercise.ExerciseId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var retrievedExercise = await response.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(retrievedExercise);
        Assert.Equal(exercise.ExerciseId, retrievedExercise.ExerciseId);
        Assert.Equal(2, retrievedExercise.MuscleGroups.Count);

        Assert.Contains(
            retrievedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == primaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Primary
        );

        Assert.Contains(
            retrievedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == secondaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Secondary
        );
    }

    [Fact]
    public async Task GetById_WhenExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/exercises/{exerciseId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WhenExercisesExist_ReturnsCreatedExercises()
    {
        // Arrange
        var primaryGroup = await CreateMuscleGroupAsync();

        var exercise = await CreateExerciseAsync(
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = primaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Primary
            }
        );

        // Act
        var response = await _client.GetAsync(
            "/api/exercises?page=1&pageSize=100"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var exercises = await response.Content
            .ReadFromJsonAsync<List<ExerciseSummaryDto>>();

        Assert.NotNull(exercises);

        var retrievedExercise = Assert.Single(
            exercises.Where(e => e.ExerciseId == exercise.ExerciseId)
        );

        Assert.Equal(exercise.Name, retrievedExercise.Name);
        Assert.Equal(primaryGroup.Name, retrievedExercise.PrimaryMuscleGroup);
    }

    [Fact]
    public async Task Update_WhenExerciseExists_UpdatesExerciseAndMuscleGroupAssociations()
    {
        // Arrange
        var originalPrimaryGroup = await CreateMuscleGroupAsync();
        var originalSecondaryGroup = await CreateMuscleGroupAsync();
        var newPrimaryGroup = await CreateMuscleGroupAsync();

        var exercise = await CreateExerciseAsync(
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = originalPrimaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Primary
            },
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = originalSecondaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Secondary
            }
        );

        var dto = new UpdateExerciseDto
        {
            Name = "Updated Bench Press",
            Description = "Updated description",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = originalPrimaryGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Secondary
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = newPrimaryGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        // Act - Update exercise
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/exercises/{exercise.ExerciseId}",
            dto
        );

        // Assert - Update response
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedExercise = await updateResponse.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(updatedExercise);
        Assert.Equal("Updated Bench Press", updatedExercise.Name);
        Assert.Equal("Updated description", updatedExercise.Description);
        Assert.Equal(2, updatedExercise.MuscleGroups.Count);

        Assert.Contains(
            updatedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == originalPrimaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Secondary
        );

        Assert.Contains(
            updatedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == newPrimaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Primary
        );

        Assert.DoesNotContain(
            updatedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == originalSecondaryGroup.MuscleGroupId
        );

        // Act - Retrieve updated exercise
        var getResponse = await _client.GetAsync(
            $"/api/exercises/{exercise.ExerciseId}"
        );

        // Assert - Updated associations were persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedExercise = await getResponse.Content
            .ReadFromJsonAsync<ExerciseDetailedDto>();

        Assert.NotNull(retrievedExercise);
        Assert.Equal("Updated Bench Press", retrievedExercise.Name);
        Assert.Equal(2, retrievedExercise.MuscleGroups.Count);

        Assert.Contains(
            retrievedExercise.MuscleGroups,
            mg => mg.MuscleGroupId == newPrimaryGroup.MuscleGroupId &&
                  mg.Role == MuscleGroupRole.Primary
        );
    }

    [Fact]
    public async Task Update_WhenExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var muscleGroup = await CreateMuscleGroupAsync();
        var exerciseId = Guid.NewGuid();

        var dto = new UpdateExerciseDto
        {
            Name = "Updated Exercise",
            Description = "Updated description",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/exercises/{exerciseId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenMuscleGroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var primaryGroup = await CreateMuscleGroupAsync();

        var exercise = await CreateExerciseAsync(
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = primaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Primary
            }
        );

        var dto = new UpdateExerciseDto
        {
            Name = "Updated Exercise",
            Description = "Updated description",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = Guid.NewGuid(),
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/exercises/{exercise.ExerciseId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExerciseExists_DeletesExerciseAndItsAssociations()
    {
        // Arrange
        var primaryGroup = await CreateMuscleGroupAsync();
        var secondaryGroup = await CreateMuscleGroupAsync();

        var exercise = await CreateExerciseAsync(
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = primaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Primary
            },
            new ExerciseMuscleGroupInputDto
            {
                MuscleGroupId = secondaryGroup.MuscleGroupId,
                Role = MuscleGroupRole.Secondary
            }
        );

        // Act - Delete exercise
        var deleteResponse = await _client.DeleteAsync(
            $"/api/exercises/{exercise.ExerciseId}"
        );

        // Assert - Deletion response
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Act - Retrieve deleted exercise
        var getResponse = await _client.GetAsync(
            $"/api/exercises/{exercise.ExerciseId}"
        );

        // Assert - Exercise no longer exists
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        // Assert - Muscle groups still exist
        var primaryGroupResponse = await _client.GetAsync(
            $"/api/muscle-groups/{primaryGroup.MuscleGroupId}"
        );

        var secondaryGroupResponse = await _client.GetAsync(
            $"/api/muscle-groups/{secondaryGroup.MuscleGroupId}"
        );

        Assert.Equal(HttpStatusCode.OK, primaryGroupResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondaryGroupResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenExerciseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/exercises/{exerciseId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

    private async Task<ExerciseDetailedDto> CreateExerciseAsync(
        params ExerciseMuscleGroupInputDto[] muscleGroups)
    {
        var dto = new CreateExerciseDto
        {
            Name = $"Test Exercise {Guid.NewGuid():N}",
            Description = "Integration test exercise",
            MuscleGroups = muscleGroups.ToList()
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
}
