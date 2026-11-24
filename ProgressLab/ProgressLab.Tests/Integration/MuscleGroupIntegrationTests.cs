using System.Net;
using System.Net.Http.Json;
using MuscleGroups;

namespace ProgressLab.Tests.Integration;

public class MuscleGroupIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MuscleGroupIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_WhenValidData_ReturnsCreatedResourceThatCanBeRetrieved()
    {
        // Arrange
        var dto = new CreateMuscleGroupDto
        {
            Name = $"Chest {Guid.NewGuid():N}",
            Description = "Chest muscles"
        };

        // Act - Create muscle group
        var createResponse = await _client.PostAsJsonAsync(
            "/api/muscle-groups",
            dto
        );

        // Assert - Creation response
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdMuscleGroup = await createResponse.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(createdMuscleGroup);
        Assert.NotEqual(Guid.Empty, createdMuscleGroup.MuscleGroupId);
        Assert.Equal(dto.Name, createdMuscleGroup.Name);
        Assert.Equal(dto.Description, createdMuscleGroup.Description);

        // Act - Retrieve created muscle group
        var getResponse = await _client.GetAsync(
            $"/api/muscle-groups/{createdMuscleGroup.MuscleGroupId}"
        );

        // Assert - Retrieval response
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedMuscleGroup = await getResponse.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(retrievedMuscleGroup);
        Assert.Equal(
            createdMuscleGroup.MuscleGroupId,
            retrievedMuscleGroup.MuscleGroupId
        );
        Assert.Equal(dto.Name, retrievedMuscleGroup.Name);
        Assert.Equal(dto.Description, retrievedMuscleGroup.Description);
    }

    [Fact]
    public async Task GetAll_WhenMuscleGroupsExist_ReturnsCreatedMuscleGroups()
    {
        // Arrange
        var createdMuscleGroup = await CreateMuscleGroupAsync();

        // Act
        var response = await _client.GetAsync(
            "/api/muscle-groups?page=1&pageSize=100"
        );

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains(
            createdMuscleGroup.MuscleGroupId.ToString(),
            content
        );
    }

    [Fact]
    public async Task GetById_WhenMuscleGroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/muscle-groups/{muscleGroupId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_WhenMuscleGroupExists_UpdatesAndPersistsValues()
    {
        // Arrange
        var createdMuscleGroup = await CreateMuscleGroupAsync();

        var dto = new UpdateMuscleGroupDto
        {
            Name = $"Updated Chest {Guid.NewGuid():N}",
            Description = "Updated chest description"
        };

        // Act - Update muscle group
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/muscle-groups/{createdMuscleGroup.MuscleGroupId}",
            dto
        );

        // Assert - Update response
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedMuscleGroup = await updateResponse.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(updatedMuscleGroup);
        Assert.Equal(createdMuscleGroup.MuscleGroupId, updatedMuscleGroup.MuscleGroupId);
        Assert.Equal(dto.Name, updatedMuscleGroup.Name);
        Assert.Equal(dto.Description, updatedMuscleGroup.Description);

        // Act - Retrieve updated muscle group
        var getResponse = await _client.GetAsync(
            $"/api/muscle-groups/{createdMuscleGroup.MuscleGroupId}"
        );

        // Assert - Updated values are persisted
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var retrievedMuscleGroup = await getResponse.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(retrievedMuscleGroup);
        Assert.Equal(dto.Name, retrievedMuscleGroup.Name);
        Assert.Equal(dto.Description, retrievedMuscleGroup.Description);
    }

    [Fact]
    public async Task Update_WhenMuscleGroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        var dto = new UpdateMuscleGroupDto
        {
            Name = "Updated Chest",
            Description = "Updated description"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/muscle-groups/{muscleGroupId}",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMuscleGroupExists_DeletesAndReturnsNoContent()
    {
        // Arrange
        var createdMuscleGroup = await CreateMuscleGroupAsync();

        // Act - Delete muscle group
        var deleteResponse = await _client.DeleteAsync(
            $"/api/muscle-groups/{createdMuscleGroup.MuscleGroupId}"
        );

        // Assert - Deletion response
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Act - Retrieve deleted muscle group
        var getResponse = await _client.GetAsync(
            $"/api/muscle-groups/{createdMuscleGroup.MuscleGroupId}"
        );

        // Assert - Resource no longer exists
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenMuscleGroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/muscle-groups/{muscleGroupId}"
        );

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WhenNameIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateMuscleGroupDto
        {
            Name = "",
            Description = "Chest muscles"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/muscle-groups",
            dto
        );

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<MuscleGroupDetailedDto> CreateMuscleGroupAsync()
    {
        var dto = new CreateMuscleGroupDto
        {
            Name = $"Test Muscle Group {Guid.NewGuid():N}",
            Description = "Integration test description"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/muscle-groups",
            dto
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdMuscleGroup = await response.Content
            .ReadFromJsonAsync<MuscleGroupDetailedDto>();

        Assert.NotNull(createdMuscleGroup);

        return createdMuscleGroup;
    }
}
