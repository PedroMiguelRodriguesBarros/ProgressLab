using MuscleGroups;
using Moq;

namespace ProgressLab.Tests.Features.MuscleGroups;

public class MuscleGroupServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenMuscleGroupExists_ReturnsMappedDto()
    {
        // Arrange
        var muscleGroup = new MuscleGroup("Chest", "Chest muscles");

        var expectedDto = new MuscleGroupDetailedDto();

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        var detailedMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();

        detailedMapperMock
            .Setup(mapper => mapper.Map(muscleGroup))
            .Returns(expectedDto);

        var service = CreateService(
            repositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(muscleGroup.MuscleGroupId);

        // Assert
        Assert.Same(expectedDto, result);

        repositoryMock.Verify(
            repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(muscleGroup),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenMuscleGroupDoesNotExist_ReturnsNull()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MuscleGroup?)null);

        var detailedMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();

        var service = CreateService(
            repositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(muscleGroupId);

        // Assert
        Assert.Null(result);

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<MuscleGroup>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllAsync_WhenMuscleGroupsExist_ReturnsMappedSummaries()
    {
        // Arrange
        var firstMuscleGroup = new MuscleGroup("Chest", "Chest muscles");
        var secondMuscleGroup = new MuscleGroup("Back", "Back muscles");

        var muscleGroups = new List<MuscleGroup>
        {
            firstMuscleGroup,
            secondMuscleGroup
        };

        var firstSummary = new MuscleGroupSummaryDto();
        var secondSummary = new MuscleGroupSummaryDto();

        var page = 1;
        var pageSize = 10;

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetAllAsync(
                page,
                pageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<MuscleGroup>)muscleGroups);

        var summaryMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupSummaryDto>>();

        summaryMapperMock
            .Setup(mapper => mapper.Map(firstMuscleGroup))
            .Returns(firstSummary);

        summaryMapperMock
            .Setup(mapper => mapper.Map(secondMuscleGroup))
            .Returns(secondSummary);

        var service = CreateService(
            repositoryMock,
            summaryMapperMock: summaryMapperMock
        );

        // Act
        var result = await service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(firstSummary, result[0]);
        Assert.Same(secondSummary, result[1]);

        repositoryMock.Verify(
            repo => repo.GetAllAsync(
                page,
                pageSize,
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(firstMuscleGroup),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(secondMuscleGroup),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_CreatesAndReturnsDetailedDto()
    {
        // Arrange
        var dto = new CreateMuscleGroupDto
        {
            Name = "Chest",
            Description = "Chest muscles"
        };

        var muscleGroup = new MuscleGroup("Chest", "Chest muscles");

        var expectedDto = new MuscleGroupDetailedDto();

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.AddAsync(
                muscleGroup,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        var createMapperMock = new Mock<IMapper<CreateMuscleGroupDto, MuscleGroup>>();

        createMapperMock
            .Setup(mapper => mapper.Map(dto))
            .Returns(muscleGroup);

        var detailedMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();

        detailedMapperMock
            .Setup(mapper => mapper.Map(muscleGroup))
            .Returns(expectedDto);

        var service = CreateService(
            repositoryMock,
            detailedMapperMock: detailedMapperMock,
            createMapperMock: createMapperMock
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Same(expectedDto, result);

        createMapperMock.Verify(
            mapper => mapper.Map(dto),
            Times.Once
        );

        repositoryMock.Verify(
            repo => repo.AddAsync(muscleGroup, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(muscleGroup),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenMuscleGroupDoesNotExist_ReturnsNull()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        var dto = new UpdateMuscleGroupDto
        {
            Name = "Updated Chest",
            Description = "Updated description"
        };

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MuscleGroup?)null);

        var detailedMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();

        var service = CreateService(
            repositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(muscleGroupId, dto);

        // Assert
        Assert.Null(result);

        repositoryMock.Verify(
            repo => repo.UpdateAsync(
                It.IsAny<MuscleGroup>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<MuscleGroup>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenMuscleGroupExists_UpdatesPropertiesAndReturnsDetailedDto()
    {
        // Arrange
        var muscleGroup = new MuscleGroup("Old Name", "Old description");

        var dto = new UpdateMuscleGroupDto
        {
            Name = "  Updated Chest  ",
            Description = "  Updated description  "
        };

        var expectedDto = new MuscleGroupDetailedDto();

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        repositoryMock
            .Setup(repo => repo.UpdateAsync(
                muscleGroup,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var detailedMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();

        detailedMapperMock
            .Setup(mapper => mapper.Map(muscleGroup))
            .Returns(expectedDto);

        var service = CreateService(
            repositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(muscleGroup.MuscleGroupId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal("Updated Chest", muscleGroup.Name);
        Assert.Equal("Updated description", muscleGroup.Description);

        repositoryMock.Verify(
            repo => repo.UpdateAsync(muscleGroup, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(muscleGroup),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenMuscleGroupDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MuscleGroup?)null);

        var service = CreateService(repositoryMock);

        // Act
        var result = await service.DeleteAsync(muscleGroupId);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            repo => repo.DeleteAsync(
                It.IsAny<MuscleGroup>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenMuscleGroupExists_ReturnsTrue()
    {
        // Arrange
        var muscleGroup = new MuscleGroup("Chest", "Chest muscles");

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        repositoryMock
            .Setup(repo => repo.DeleteAsync(
                muscleGroup,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService(repositoryMock);

        // Act
        var result = await service.DeleteAsync(muscleGroup.MuscleGroupId);

        // Assert
        Assert.True(result);

        repositoryMock.Verify(
            repo => repo.DeleteAsync(muscleGroup, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryCannotDeleteMuscleGroup_ReturnsFalse()
    {
        // Arrange
        var muscleGroup = new MuscleGroup("Chest", "Chest muscles");

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        repositoryMock
            .Setup(repo => repo.DeleteAsync(
                muscleGroup,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService(repositoryMock);

        // Act
        var result = await service.DeleteAsync(muscleGroup.MuscleGroupId);

        // Assert
        Assert.False(result);

        repositoryMock.Verify(
            repo => repo.DeleteAsync(muscleGroup, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenDescriptionIsNull_UpdatesMuscleGroupWithNullDescription()
    {
        // Arrange
        var muscleGroup = new MuscleGroup("Chest", "Old description");

        var dto = new UpdateMuscleGroupDto
        {
            Name = "Chest",
            Description = null
        };

        var expectedDto = new MuscleGroupDetailedDto();

        var repositoryMock = new Mock<IMuscleGroupRepository>();

        repositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        repositoryMock
            .Setup(repo => repo.UpdateAsync(
                muscleGroup,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var detailedMapperMock = new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();

        detailedMapperMock
            .Setup(mapper => mapper.Map(muscleGroup))
            .Returns(expectedDto);

        var service = CreateService(
            repositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(muscleGroup.MuscleGroupId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal("Chest", muscleGroup.Name);
        Assert.Null(muscleGroup.Description);

        repositoryMock.Verify(
            repo => repo.UpdateAsync(muscleGroup, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(muscleGroup),
            Times.Once
        );
    }

    private static MuscleGroupService CreateService(
        Mock<IMuscleGroupRepository> repositoryMock,
        Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>? detailedMapperMock = null,
        Mock<IMapper<MuscleGroup, MuscleGroupSummaryDto>>? summaryMapperMock = null,
        Mock<IMapper<CreateMuscleGroupDto, MuscleGroup>>? createMapperMock = null)
    {
        detailedMapperMock ??= new Mock<IMapper<MuscleGroup, MuscleGroupDetailedDto>>();
        summaryMapperMock ??= new Mock<IMapper<MuscleGroup, MuscleGroupSummaryDto>>();
        createMapperMock ??= new Mock<IMapper<CreateMuscleGroupDto, MuscleGroup>>();

        return new MuscleGroupService(
            repositoryMock.Object,
            detailedMapperMock.Object,
            summaryMapperMock.Object,
            createMapperMock.Object
        );
    }
}
