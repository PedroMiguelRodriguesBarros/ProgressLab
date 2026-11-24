
using Exercises;
using Moq;
using Workouts;

namespace ProgressLab.Tests.Features.Workouts;

public class WorkoutServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenWorkoutDoesNotExist_ReturnsNull()
    {
        // Arrange
        var workoutId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(workoutId);

        // Assert
        Assert.Null(result);

        workoutRepositoryMock.Verify(
            repo => repo.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Workout>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllAsync_WhenWorkoutsExist_ReturnsMappedSummaries()
    {
        // Arrange
        var firstWorkout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var secondWorkout = new Workout("Lower Body", DateTimeOffset.UtcNow, null);

        var workouts = new List<Workout> { firstWorkout, secondWorkout };

        var firstSummary = new WorkoutSummaryDto
        {
            WorkoutId = firstWorkout.WorkoutId,
            Name = firstWorkout.Name,
            Date = firstWorkout.Date
        };

        var secondSummary = new WorkoutSummaryDto
        {
            WorkoutId = secondWorkout.WorkoutId,
            Name = secondWorkout.Name,
            Date = secondWorkout.Date
        };

        var page = 1;
        var pageSize = 10;

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                page,
                pageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Workout>)workouts);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var summaryMapperMock = new Mock<IMapper<Workout, WorkoutSummaryDto>>();
        summaryMapperMock
            .Setup(mapper => mapper.Map(firstWorkout))
            .Returns(firstSummary);

        summaryMapperMock
            .Setup(mapper => mapper.Map(secondWorkout))
            .Returns(secondSummary);

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            summaryMapperMock: summaryMapperMock
        );

        // Act
        var result = await service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(firstSummary, result[0]);
        Assert.Same(secondSummary, result[1]);

        workoutRepositoryMock.Verify(
            repo => repo.GetAllAsync(page, pageSize, It.IsAny<CancellationToken>()),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(firstWorkout),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(secondWorkout),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_WhenWorkoutIsValid_CreatesAndReturnsDetailedDto()
    {
        // Arrange
        var date = DateTimeOffset.UtcNow;

        var dto = new CreateWorkoutDto
        {
            Name = "Upper Body",
            Date = date,
            Notes = "Training session"
        };

        var workout = new Workout("Upper Body", date, "Training session");

        var expectedDto = new WorkoutDetailedDto
        {
            WorkoutId = workout.WorkoutId,
            Name = workout.Name,
            Date = workout.Date,
            Notes = workout.Notes
        };

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.AddAsync(
                workout,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var createMapperMock = new Mock<IMapper<CreateWorkoutDto, Workout>>();
        createMapperMock
            .Setup(mapper => mapper.Map(dto))
            .Returns(workout);

        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(workout))
            .Returns(expectedDto);

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            createMapperMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Same(expectedDto, result);

        createMapperMock.Verify(
            mapper => mapper.Map(dto),
            Times.Once
        );

        workoutRepositoryMock.Verify(
            repo => repo.AddAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(workout),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenWorkoutDoesNotExist_ReturnsNull()
    {
        // Arrange
        var workoutId = Guid.NewGuid();

        var dto = new UpdateWorkoutDto
        {
            Name = "Updated Workout",
            Notes = "Updated notes"
        };

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(workoutId, dto);

        // Assert
        Assert.Null(result);

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        workoutRepositoryMock.Verify(
            repo => repo.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()),
            Times.Never
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Workout>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenWorkoutExists_UpdatesPropertiesAndReturnsDetailedDto()
    {
        // Arrange
        var originalDate = DateTimeOffset.UtcNow.AddDays(-2);
        var updatedDate = DateTimeOffset.UtcNow;

        var workout = new Workout(
            "Old Workout",
            originalDate,
            "Old notes"
        );

        var workoutId = workout.WorkoutId;

        var dto = new UpdateWorkoutDto
        {
            Name = "  Updated Workout  ",
            Date = updatedDate,
            Notes = "  Updated notes  "
        };

        var expectedDto = new WorkoutDetailedDto
        {
            WorkoutId = workoutId,
            Name = "Updated Workout",
            Date = updatedDate,
            Notes = "Updated notes"
        };

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        workoutRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        workoutRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                workout,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(workout))
            .Returns(expectedDto);

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(workoutId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal("Updated Workout", workout.Name);
        Assert.Equal(updatedDate, workout.Date);
        Assert.Equal("Updated notes", workout.Notes);

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );

        workoutRepositoryMock.Verify(
            repo => repo.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(workout),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenDateIsNotProvided_KeepsExistingDate()
    {
        // Arrange
        var originalDate = DateTimeOffset.UtcNow.AddDays(-5);

        var workout = new Workout(
            "Old Workout",
            originalDate,
            "Old notes"
        );

        var workoutId = workout.WorkoutId;

        var dto = new UpdateWorkoutDto
        {
            Name = "Updated Workout",
            Date = null,
            Notes = null
        };

        var expectedDto = new WorkoutDetailedDto
        {
            WorkoutId = workoutId,
            Name = "Updated Workout",
            Date = originalDate,
            Notes = null
        };

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        workoutRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(workout))
            .Returns(expectedDto);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(workoutId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal(originalDate, workout.Date);
        Assert.Equal("Updated Workout", workout.Name);
        Assert.Null(workout.Notes);

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenWorkoutDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var workoutId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act
        var result = await service.DeleteAsync(workoutId);

        // Assert
        Assert.False(result);

        workoutRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenWorkoutExists_ReturnsTrue()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var workoutId = workout.WorkoutId;

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        workoutRepositoryMock
            .Setup(repo => repo.DeleteAsync(
                workout,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act
        var result = await service.DeleteAsync(workoutId);

        // Assert
        Assert.True(result);

        workoutRepositoryMock.Verify(
            repo => repo.DeleteAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryCannotDeleteWorkout_ReturnsFalse()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var workoutId = workout.WorkoutId;

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        workoutRepositoryMock
            .Setup(repo => repo.DeleteAsync(
                workout,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act
        var result = await service.DeleteAsync(workoutId);

        // Assert
        Assert.False(result);

        workoutRepositoryMock.Verify(
            repo => repo.DeleteAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task AddExerciseAsync_WhenWorkoutDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddExerciseAsync(workoutId, exerciseId)
        );

        exerciseRepositoryMock.Verify(
            repo => repo.ExistsAsync(exerciseId, It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AddExerciseAsync_WhenExerciseDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var exerciseId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.ExistsAsync(
                exerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.AddExerciseAsync(workout.WorkoutId, exerciseId)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AddExerciseAsync_WhenExerciseAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, exerciseId, 1)
        );

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.ExistsAsync(
                exerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddExerciseAsync(workout.WorkoutId, exerciseId)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AddExerciseAsync_WhenExerciseIsValid_AddsExerciseWithNextOrder()
    {
        // Arrange
        var firstExerciseId = Guid.NewGuid();
        var secondExerciseId = Guid.NewGuid();
        var newExerciseId = Guid.NewGuid();

        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, firstExerciseId, 1)
        );

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, secondExerciseId, 2)
        );

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.ExistsAsync(
                newExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act
        await service.AddExerciseAsync(workout.WorkoutId, newExerciseId);

        // Assert
        var addedExercise = Assert.Single(
            workout.WorkoutExercises.Where(we => we.ExerciseId == newExerciseId)
        );

        Assert.Equal(3, addedExercise.Order);

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task RemoveExerciseAsync_WhenWorkoutDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RemoveExerciseAsync(workoutId, exerciseId)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RemoveExerciseAsync_WhenExerciseIsNotPartOfWorkout_ThrowsInvalidOperationException()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var exerciseId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RemoveExerciseAsync(workout.WorkoutId, exerciseId)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RemoveExerciseAsync_WhenExerciseExists_RemovesAndReordersRemainingExercises()
    {
        // Arrange
        var firstExerciseId = Guid.NewGuid();
        var exerciseToRemoveId = Guid.NewGuid();
        var thirdExerciseId = Guid.NewGuid();

        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, firstExerciseId, 1)
        );

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, exerciseToRemoveId, 2)
        );

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, thirdExerciseId, 3)
        );

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act
        await service.RemoveExerciseAsync(workout.WorkoutId, exerciseToRemoveId);

        // Assert
        var remainingExercises = workout.WorkoutExercises
            .OrderBy(we => we.Order)
            .ToList();

        Assert.Equal(2, remainingExercises.Count);

        Assert.Equal(
            new[] { firstExerciseId, thirdExerciseId },
            remainingExercises.Select(we => we.ExerciseId)
        );

        Assert.Equal(
            new[] { 1, 2 },
            remainingExercises.Select(we => we.Order)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateExerciseOrderAsync_WhenWorkoutDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var workoutId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.UpdateExerciseOrderAsync(workoutId, exerciseId, 1)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateExerciseOrderAsync(
                It.IsAny<Workout>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateExerciseOrderAsync_WhenExerciseIsNotPartOfWorkout_ThrowsInvalidOperationException()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var exerciseId = Guid.NewGuid();

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateExerciseOrderAsync(workout.WorkoutId, exerciseId, 1)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateExerciseOrderAsync(
                It.IsAny<Workout>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task UpdateExerciseOrderAsync_WhenNewOrderIsOutsideValidRange_ThrowsArgumentOutOfRangeException(int newOrder)
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);
        var exerciseIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        for (int i = 0; i < exerciseIds.Length; i++)
        {
            workout.WorkoutExercises.Add(
                new WorkoutExercise(workout.WorkoutId, exerciseIds[i], i + 1)
            );
        }

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.UpdateExerciseOrderAsync(
                workout.WorkoutId,
                exerciseIds[0],
                newOrder)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateExerciseOrderAsync(
                It.IsAny<Workout>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateExerciseOrderAsync_WhenNewOrderIsValid_ReordersExercises()
    {
        // Arrange
        var firstExerciseId = Guid.NewGuid();
        var secondExerciseId = Guid.NewGuid();
        var thirdExerciseId = Guid.NewGuid();

        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, firstExerciseId, 1)
        );

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, secondExerciseId, 2)
        );

        workout.WorkoutExercises.Add(
            new WorkoutExercise(workout.WorkoutId, thirdExerciseId, 3)
        );

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var service = CreateService(workoutRepositoryMock, exerciseRepositoryMock);

        // Act
        await service.UpdateExerciseOrderAsync(workout.WorkoutId, thirdExerciseId, 1);

        // Assert
        var reorderedExercises = workout.WorkoutExercises
            .OrderBy(we => we.Order)
            .ToList();

        Assert.Equal(
            new[] { thirdExerciseId, firstExerciseId, secondExerciseId },
            reorderedExercises.Select(we => we.ExerciseId)
        );

        Assert.Equal(
            new[] { 1, 2, 3 },
            reorderedExercises.Select(we => we.Order)
        );

        workoutRepositoryMock.Verify(
            repo => repo.UpdateExerciseOrderAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenWorkoutExists_ReturnsMappedDto()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);

        var expectedDto = new WorkoutDetailedDto
        {
            WorkoutId = workout.WorkoutId,
            Name = workout.Name,
            Date = workout.Date,
            Notes = workout.Notes
        };

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();
        workoutRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(workout))
            .Returns(expectedDto);

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(workout.WorkoutId);

        // Assert
        Assert.Same(expectedDto, result);

        workoutRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(workout),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdatedWorkoutCannotBeRetrieved_ReturnsNull()
    {
        // Arrange
        var workout = new Workout("Upper Body", DateTimeOffset.UtcNow, null);

        var dto = new UpdateWorkoutDto
        {
            Name = "Updated Upper Body",
            Date = DateTimeOffset.UtcNow,
            Notes = "Updated notes"
        };

        var workoutRepositoryMock = new Mock<IWorkoutRepository>();

        workoutRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        workoutRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                workout,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        workoutRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                workout.WorkoutId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var detailedMapperMock = new Mock<IMapper<Workout, WorkoutDetailedDto>>();

        var service = CreateService(
            workoutRepositoryMock,
            exerciseRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(workout.WorkoutId, dto);

        // Assert
        Assert.Null(result);

        Assert.Equal("Updated Upper Body", workout.Name);

        workoutRepositoryMock.Verify(
            repo => repo.UpdateAsync(workout, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Workout>()),
            Times.Never
        );
    }


    private static WorkoutService CreateService(
        Mock<IWorkoutRepository> workoutRepositoryMock,
        Mock<IExerciseRepository> exerciseRepositoryMock,
        Mock<IMapper<CreateWorkoutDto, Workout>>? createMapperMock = null,
        Mock<IMapper<Workout, WorkoutSummaryDto>>? summaryMapperMock = null,
        Mock<IMapper<Workout, WorkoutDetailedDto>>? detailedMapperMock = null)
    {
        createMapperMock ??= new Mock<IMapper<CreateWorkoutDto, Workout>>();
        summaryMapperMock ??= new Mock<IMapper<Workout, WorkoutSummaryDto>>();
        detailedMapperMock ??= new Mock<IMapper<Workout, WorkoutDetailedDto>>();

        return new WorkoutService(
            workoutRepositoryMock.Object,
            exerciseRepositoryMock.Object,
            createMapperMock.Object,
            summaryMapperMock.Object,
            detailedMapperMock.Object
        );
    }
}
