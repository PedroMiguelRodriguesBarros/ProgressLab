
using Moq;
using Sets;
using Workouts;

namespace ProgressLab.Tests.Features.Sets;

public class SetServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenSetExists_ReturnsMappedDto()
    {
        // Arrange
        var set = new Set(1, 10, 50m);

        var expectedDto = new SetDetailedDto
        {
            SetId = set.SetId,
            SetNumber = set.SetNumber,
            Repetitions = set.Repetitions,
            Weight = set.Weight
        };

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                set.SetId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(set);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(set))
            .Returns(expectedDto);

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(set.SetId);

        // Assert
        Assert.Same(expectedDto, result);

        setRepositoryMock.Verify(
            repo => repo.GetByIdAsync(set.SetId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(set),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenSetDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                setId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Set?)null);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetByIdAsync(setId)
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Set>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllAsync_WhenSetsExist_ReturnsMappedSummaries()
    {
        // Arrange
        var firstSet = new Set(1, 10, 50m);
        var secondSet = new Set(2, 8, 55m);

        var sets = new List<Set> { firstSet, secondSet };

        var firstSummary = new SetSummaryDto
        {
            SetNumber = firstSet.SetNumber,
            Repetitions = firstSet.Repetitions
        };

        var secondSummary = new SetSummaryDto
        {
            SetNumber = secondSet.SetNumber,
            Repetitions = secondSet.Repetitions
        };

        var page = 1;
        var pageSize = 10;

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                page,
                pageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Set>)sets);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        var summaryMapperMock = new Mock<IMapper<Set, SetSummaryDto>>();
        summaryMapperMock
            .Setup(mapper => mapper.Map(firstSet))
            .Returns(firstSummary);

        summaryMapperMock
            .Setup(mapper => mapper.Map(secondSet))
            .Returns(secondSummary);

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            summaryMapperMock: summaryMapperMock
        );

        // Act
        var result = await service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(firstSummary, result[0]);
        Assert.Same(secondSummary, result[1]);

        setRepositoryMock.Verify(
            repo => repo.GetAllAsync(page, pageSize, It.IsAny<CancellationToken>()),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(firstSet),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(secondSet),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllByWorkoutExerciseIdAsync_WhenWorkoutExerciseDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var workoutExerciseId = Guid.NewGuid();

        var setRepositoryMock = new Mock<ISetRepository>();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutExercise?)null);

        var summaryMapperMock = new Mock<IMapper<Set, SetSummaryDto>>();

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            summaryMapperMock: summaryMapperMock
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetAllByWorkoutExerciseIdAsync(workoutExerciseId)
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Set>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllByWorkoutExerciseIdAsync_WhenSetsExist_ReturnsOrderedSummaries()
    {
        // Arrange
        var workoutExercise = new WorkoutExercise
        {
            WorkoutExerciseId = Guid.NewGuid()
        };

        var firstSet = new Set(1, 12, 40m);
        var secondSet = new Set(2, 10, 45m);
        var thirdSet = new Set(3, 8, 50m);

        // Deliberately add the sets in a different order.
        workoutExercise.Sets.Add(thirdSet);
        workoutExercise.Sets.Add(firstSet);
        workoutExercise.Sets.Add(secondSet);

        var firstSummary = new SetSummaryDto
        {
            SetNumber = 1,
            Repetitions = 12
        };

        var secondSummary = new SetSummaryDto
        {
            SetNumber = 2,
            Repetitions = 10
        };

        var thirdSummary = new SetSummaryDto
        {
            SetNumber = 3,
            Repetitions = 8
        };

        var setRepositoryMock = new Mock<ISetRepository>();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExercise.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workoutExercise);

        var summaryMapperMock = new Mock<IMapper<Set, SetSummaryDto>>();
        summaryMapperMock
            .Setup(mapper => mapper.Map(firstSet))
            .Returns(firstSummary);

        summaryMapperMock
            .Setup(mapper => mapper.Map(secondSet))
            .Returns(secondSummary);

        summaryMapperMock
            .Setup(mapper => mapper.Map(thirdSet))
            .Returns(thirdSummary);

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            summaryMapperMock: summaryMapperMock
        );

        // Act
        var result = await service.GetAllByWorkoutExerciseIdAsync(
            workoutExercise.WorkoutExerciseId
        );

        // Assert
        Assert.Equal(3, result.Count);

        Assert.Equal(
            new[] { 1, 2, 3 },
            result.Select(s => s.SetNumber)
        );

        Assert.Same(firstSummary, result[0]);
        Assert.Same(secondSummary, result[1]);
        Assert.Same(thirdSummary, result[2]);
    }

    [Fact]
    public async Task CreateAsync_WhenWorkoutExerciseDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var workoutExerciseId = Guid.NewGuid();

        var dto = new CreateSetDto
        {
            Repetitions = 10,
            Weight = 50m
        };

        var setRepositoryMock = new Mock<ISetRepository>();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutExercise?)null);

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateAsync(workoutExerciseId, dto)
        );

        setRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Set>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Set>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenSetsExist_CreatesSetWithNextNumber()
    {
        // Arrange
        var workoutExercise = new WorkoutExercise
        {
            WorkoutExerciseId = Guid.NewGuid()
        };

        var firstSet = new Set(1, 12, 40m);
        var thirdSet = new Set(3, 8, 50m);

        firstSet.WorkoutExerciseId = workoutExercise.WorkoutExerciseId;
        thirdSet.WorkoutExerciseId = workoutExercise.WorkoutExerciseId;

        workoutExercise.Sets.Add(firstSet);
        workoutExercise.Sets.Add(thirdSet);

        var dto = new CreateSetDto
        {
            Repetitions = 6,
            Weight = 55m
        };

        var expectedDto = new SetDetailedDto
        {
            SetNumber = 4,
            Repetitions = 6,
            Weight = 55m
        };

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<Set>(),
                It.IsAny<CancellationToken>()))
            .Returns((Set set, CancellationToken _) => Task.FromResult(set));

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExercise.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workoutExercise);

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(It.IsAny<Set>()))
            .Returns(expectedDto);

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.CreateAsync(workoutExercise.WorkoutExerciseId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        var createdSet = Assert.Single(
            workoutExercise.Sets.Where(s => s.SetNumber == 4)
        );

        Assert.Equal(6, createdSet.Repetitions);
        Assert.Equal(55m, createdSet.Weight);
        Assert.Equal(workoutExercise.WorkoutExerciseId, createdSet.WorkoutExerciseId);

        setRepositoryMock.Verify(
            repo => repo.AddAsync(
                It.Is<Set>(s =>
                    s.SetNumber == 4 &&
                    s.Repetitions == 6 &&
                    s.Weight == 55m &&
                    s.WorkoutExerciseId == workoutExercise.WorkoutExerciseId),
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(createdSet),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_WhenWorkoutExerciseHasNoSets_StartsSetNumberAtOne()
    {
        // Arrange
        var workoutExercise = new WorkoutExercise
        {
            WorkoutExerciseId = Guid.NewGuid()
        };

        var dto = new CreateSetDto
        {
            Repetitions = 12,
            Weight = 30m
        };

        var expectedDto = new SetDetailedDto
        {
            SetNumber = 1,
            Repetitions = 12,
            Weight = 30m
        };

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.AddAsync(
                It.IsAny<Set>(),
                It.IsAny<CancellationToken>()))
            .Returns((Set set, CancellationToken _) => Task.FromResult(set));

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExercise.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workoutExercise);

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(It.IsAny<Set>()))
            .Returns(expectedDto);

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.CreateAsync(workoutExercise.WorkoutExerciseId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        var createdSet = Assert.Single(workoutExercise.Sets);

        Assert.Equal(1, createdSet.SetNumber);
        Assert.Equal(12, createdSet.Repetitions);
        Assert.Equal(30m, createdSet.Weight);
    }

    [Fact]
    public async Task UpdateSetAsync_WhenSetDoesNotExist_ReturnsNull()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var dto = new UpdateSetDto
        {
            Repetitions = 12,
            Weight = 60m
        };

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                setId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Set?)null);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateSetAsync(setId, dto);

        // Assert
        Assert.Null(result);

        setRepositoryMock.Verify(
            repo => repo.UpdateAsync(It.IsAny<Set>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Set>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateSetAsync_WhenSetExists_UpdatesPropertiesAndReturnsDetailedDto()
    {
        // Arrange
        var set = new Set(1, 10, 50m);

        var dto = new UpdateSetDto
        {
            Repetitions = 8,
            Weight = 60m
        };

        var expectedDto = new SetDetailedDto
        {
            SetId = set.SetId,
            SetNumber = set.SetNumber,
            Repetitions = 8,
            Weight = 60m
        };

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                set.SetId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(set);

        var detailedMapperMock = new Mock<IMapper<Set, SetDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(set))
            .Returns(expectedDto);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        var service = CreateService(
            setRepositoryMock,
            workoutRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateSetAsync(set.SetId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal(8, set.Repetitions);
        Assert.Equal(60m, set.Weight);

        setRepositoryMock.Verify(
            repo => repo.UpdateAsync(set, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(set),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenSetDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var setId = Guid.NewGuid();

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                setId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Set?)null);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        var service = CreateService(setRepositoryMock, workoutRepositoryMock);

        // Act
        var result = await service.DeleteAsync(setId);

        // Assert
        Assert.False(result);

        workoutRepositoryMock.Verify(
            repo => repo.GetWorkoutExerciseByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        setRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<Set>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenWorkoutExerciseDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var set = new Set(1, 10, 50m);

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                set.SetId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(set);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                set.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkoutExercise?)null);

        var service = CreateService(setRepositoryMock, workoutRepositoryMock);

        // Act
        var result = await service.DeleteAsync(set.SetId);

        // Assert
        Assert.False(result);

        setRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<Set>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenSetIsNotPartOfWorkoutExercise_ReturnsFalse()
    {
        // Arrange
        var set = new Set(1, 10, 50m);

        var workoutExercise = new WorkoutExercise
        {
            WorkoutExerciseId = set.WorkoutExerciseId
        };

        workoutExercise.Sets.Add(new Set(1, 8, 40m));

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                set.SetId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(set);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                set.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workoutExercise);

        var service = CreateService(setRepositoryMock, workoutRepositoryMock);

        // Act
        var result = await service.DeleteAsync(set.SetId);

        // Assert
        Assert.False(result);

        setRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<Set>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenSetExists_DeletesAndReordersRemainingSets()
    {
        // Arrange
        var firstSet = new Set(1, 12, 40m);
        var setToDelete = new Set(2, 10, 45m);
        var thirdSet = new Set(3, 8, 50m);

        var workoutExercise = new WorkoutExercise
        {
            WorkoutExerciseId = Guid.NewGuid()
        };

        firstSet.WorkoutExerciseId = workoutExercise.WorkoutExerciseId;
        setToDelete.WorkoutExerciseId = workoutExercise.WorkoutExerciseId;
        thirdSet.WorkoutExerciseId = workoutExercise.WorkoutExerciseId;

        workoutExercise.Sets.Add(firstSet);
        workoutExercise.Sets.Add(setToDelete);
        workoutExercise.Sets.Add(thirdSet);

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                setToDelete.SetId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(setToDelete);

        setRepositoryMock
            .Setup(repo => repo.DeleteAsync(
                setToDelete,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExercise.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workoutExercise);

        var service = CreateService(setRepositoryMock, workoutRepositoryMock);

        // Act
        var result = await service.DeleteAsync(setToDelete.SetId);

        // Assert
        Assert.True(result);

        Assert.Equal(2, workoutExercise.Sets.Count);

        var remainingSets = workoutExercise.Sets
            .OrderBy(s => s.SetNumber)
            .ToList();

        Assert.Equal(
            new[] { firstSet.SetId, thirdSet.SetId },
            remainingSets.Select(s => s.SetId)
        );

        Assert.Equal(
            new[] { 1, 2 },
            remainingSets.Select(s => s.SetNumber)
        );

        setRepositoryMock.Verify(
            repo => repo.DeleteAsync(setToDelete, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryCannotDeleteSet_ReturnsFalse()
    {
        // Arrange
        var set = new Set(1, 10, 50m);

        var workoutExercise = new WorkoutExercise
        {
            WorkoutExerciseId = Guid.NewGuid()
        };

        set.WorkoutExerciseId = workoutExercise.WorkoutExerciseId;
        workoutExercise.Sets.Add(set);

        var setRepositoryMock = new Mock<ISetRepository>();
        setRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                set.SetId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(set);

        setRepositoryMock
            .Setup(repo => repo.DeleteAsync(
                set,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetWorkoutExerciseByIdAsync(
                workoutExercise.WorkoutExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workoutExercise);

        var service = CreateService(setRepositoryMock, workoutRepositoryMock);

        // Act
        var result = await service.DeleteAsync(set.SetId);

        // Assert
        Assert.False(result);

        setRepositoryMock.Verify(
            repo => repo.DeleteAsync(set, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    private static SetService CreateService(
        Mock<ISetRepository> setRepositoryMock,
        Mock<IWorkoutRepository> workoutRepositoryMock,
        Mock<IMapper<Set, SetSummaryDto>>? summaryMapperMock = null,
        Mock<IMapper<Set, SetDetailedDto>>? detailedMapperMock = null)
    {
        summaryMapperMock ??= new Mock<IMapper<Set, SetSummaryDto>>();
        detailedMapperMock ??= new Mock<IMapper<Set, SetDetailedDto>>();

        return new SetService(
            setRepositoryMock.Object,
            workoutRepositoryMock.Object,
            summaryMapperMock.Object,
            detailedMapperMock.Object
        );
    }
}
