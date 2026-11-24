using Exercises;
using ExercisesMuscleGroups;
using MuscleGroups;
using Moq;

namespace ProgressLab.Tests.Features.Exercises;

public class ExerciseServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenExerciseExists_ReturnsMappedDto()
    {
        // Arrange
        var exercise = new Exercise("Bench Press", "Chest exercise");

        var expectedDto = new ExerciseDetailedDto
        {
            ExerciseId = exercise.ExerciseId,
            Name = exercise.Name,
            Description = exercise.Description!
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var detailedMapperMock = new Mock<IMapper<Exercise, ExerciseDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(exercise))
            .Returns(expectedDto);

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(exercise.ExerciseId);

        // Assert
        Assert.Same(expectedDto, result);

        exerciseRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(exercise),
            Times.Once
        );
    }

    [Fact]
    public async Task GetByIdAsync_WhenExerciseDoesNotExist_ReturnsNull()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var detailedMapperMock = new Mock<IMapper<Exercise, ExerciseDetailedDto>>();

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.GetByIdAsync(exerciseId);

        // Assert
        Assert.Null(result);

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Exercise>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetAllAsync_WhenExercisesExist_ReturnsMappedSummaries()
    {
        // Arrange
        var firstExercise = new Exercise("Bench Press", "Chest exercise");
        var secondExercise = new Exercise("Squat", "Leg exercise");

        var exercises = new List<Exercise>
        {
            firstExercise,
            secondExercise
        };

        var firstSummary = new ExerciseSummaryDto
        {
            ExerciseId = firstExercise.ExerciseId,
            Name = firstExercise.Name,
            PrimaryMuscleGroup = "Chest"
        };

        var secondSummary = new ExerciseSummaryDto
        {
            ExerciseId = secondExercise.ExerciseId,
            Name = secondExercise.Name,
            PrimaryMuscleGroup = "Quadriceps"
        };

        var page = 1;
        var pageSize = 10;

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetAllAsync(
                page,
                pageSize,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<Exercise>)exercises);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var summaryMapperMock = new Mock<IMapper<Exercise, ExerciseSummaryDto>>();
        summaryMapperMock
            .Setup(mapper => mapper.Map(firstExercise))
            .Returns(firstSummary);

        summaryMapperMock
            .Setup(mapper => mapper.Map(secondExercise))
            .Returns(secondSummary);

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            summaryMapperMock: summaryMapperMock
        );

        // Act
        var result = await service.GetAllAsync(page, pageSize);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Same(firstSummary, result[0]);
        Assert.Same(secondSummary, result[1]);

        exerciseRepositoryMock.Verify(
            repo => repo.GetAllAsync(page, pageSize, It.IsAny<CancellationToken>()),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(firstExercise),
            Times.Once
        );

        summaryMapperMock.Verify(
            mapper => mapper.Map(secondExercise),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_WhenMuscleGroupsAreEmpty_ReturnsNull()
    {
        // Arrange
        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var createMapperMock = new Mock<IMapper<CreateExerciseDto, Exercise>>();

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            createMapperMock: createMapperMock
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Null(result);

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        exerciseRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        createMapperMock.Verify(
            mapper => mapper.Map(dto),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenNoPrimaryMuscleGroupIsProvided_ReturnsNull()
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
                    Role = MuscleGroupRole.Secondary
                }
            }
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Null(result);

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        exerciseRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenMultiplePrimaryMuscleGroupsAreProvided_ReturnsNull()
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
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = Guid.NewGuid(),
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Null(result);

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenMuscleGroupsAreDuplicated_ReturnsNull()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroupId,
                    Role = MuscleGroupRole.Primary
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroupId,
                    Role = MuscleGroupRole.Secondary
                }
            }
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Null(result);

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenInvalidMuscleGroupRoleIsProvided_ReturnsNull()
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
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = Guid.NewGuid(),
                    Role = MuscleGroupRole.Unspecified
                }
            }
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Null(result);

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenMuscleGroupDoesNotExist_ReturnsNull()
    {
        // Arrange
        var muscleGroupId = Guid.NewGuid();

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MuscleGroup?)null);

        var createMapperMock = new Mock<IMapper<CreateExerciseDto, Exercise>>();

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            createMapperMock: createMapperMock
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Null(result);

        exerciseRepositoryMock.Verify(
            repo => repo.AddAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        createMapperMock.Verify(
            mapper => mapper.Map(dto),
            Times.Never
        );
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_CreatesExerciseWithMuscleGroups()
    {
        // Arrange
        var primaryMuscleGroup = new MuscleGroup("Chest", "Chest muscles");
        var secondaryMuscleGroup = new MuscleGroup("Triceps", "Triceps muscles");

        var dto = new CreateExerciseDto
        {
            Name = "Bench Press",
            Description = "Chest exercise",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = primaryMuscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                },
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = secondaryMuscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Secondary
                }
            }
        };

        var exercise = new Exercise(dto.Name, dto.Description);

        var expectedDto = new ExerciseDetailedDto
        {
            ExerciseId = exercise.ExerciseId,
            Name = exercise.Name,
            Description = exercise.Description!
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.AddAsync(exercise, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                primaryMuscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(primaryMuscleGroup);

        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                secondaryMuscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(secondaryMuscleGroup);

        var createMapperMock = new Mock<IMapper<CreateExerciseDto, Exercise>>();
        createMapperMock
            .Setup(mapper => mapper.Map(dto))
            .Returns(exercise);

        var detailedMapperMock = new Mock<IMapper<Exercise, ExerciseDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(exercise))
            .Returns(expectedDto);

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            detailedMapperMock,
            createMapperMock: createMapperMock
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal(2, exercise.ExerciseMuscleGroups.Count);

        Assert.Contains(
            exercise.ExerciseMuscleGroups,
            emg => emg.MuscleGroupId == primaryMuscleGroup.MuscleGroupId &&
                   emg.Role == MuscleGroupRole.Primary
        );

        Assert.Contains(
            exercise.ExerciseMuscleGroups,
            emg => emg.MuscleGroupId == secondaryMuscleGroup.MuscleGroupId &&
                   emg.Role == MuscleGroupRole.Secondary
        );

        exerciseRepositoryMock.Verify(
            repo => repo.AddAsync(exercise, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(exercise),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenMuscleGroupsAreInvalid_ReturnsNull()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

        var dto = new UpdateExerciseDto
        {
            Name = "Updated Exercise",
            Description = "Updated description",
            MuscleGroups = new()
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.UpdateAsync(exerciseId, dto);

        // Assert
        Assert.Null(result);

        exerciseRepositoryMock.Verify(
            repo => repo.GetByIdForUpdateAsync(
                exerciseId,
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenExerciseDoesNotExist_ReturnsNull()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

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

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                exerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.UpdateAsync(exerciseId, dto);

        // Assert
        Assert.Null(result);

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
        );

        exerciseRepositoryMock.Verify(
            repo => repo.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenMuscleGroupDoesNotExist_ReturnsNull()
    {
        // Arrange
        var exercise = new Exercise("Bench Press", "Old description");
        var muscleGroupId = Guid.NewGuid();

        var dto = new UpdateExerciseDto
        {
            Name = "Updated Exercise",
            Description = "Updated description",
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((MuscleGroup?)null);

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.UpdateAsync(exercise.ExerciseId, dto);

        // Assert
        Assert.Null(result);

        Assert.Equal("Bench Press", exercise.Name);
        Assert.Equal("Old description", exercise.Description);

        exerciseRepositoryMock.Verify(
            repo => repo.UpdateAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenDataIsValid_UpdatesExerciseAndMuscleGroupAssociations()
    {
        // Arrange
        var exercise = new Exercise("Old Name", "Old description");

        var originalPrimaryGroup = new MuscleGroup("Chest", "Chest muscles");
        var originalSecondaryGroup = new MuscleGroup("Triceps", "Triceps muscles");
        var newPrimaryGroup = new MuscleGroup("Shoulders", "Shoulder muscles");

        exercise.ExerciseMuscleGroups.Add(
            new ExerciseMuscleGroup(
                exercise,
                originalPrimaryGroup,
                MuscleGroupRole.Primary
            )
        );

        exercise.ExerciseMuscleGroups.Add(
            new ExerciseMuscleGroup(
                exercise,
                originalSecondaryGroup,
                MuscleGroupRole.Secondary
            )
        );

        var dto = new UpdateExerciseDto
        {
            Name = "  Updated Name  ",
            Description = "  Updated description  ",
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

        var expectedDto = new ExerciseDetailedDto
        {
            ExerciseId = exercise.ExerciseId,
            Name = "Updated Name",
            Description = "Updated description"
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                exercise,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                originalPrimaryGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(originalPrimaryGroup);

        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                newPrimaryGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(newPrimaryGroup);

        var detailedMapperMock = new Mock<IMapper<Exercise, ExerciseDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(exercise))
            .Returns(expectedDto);

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(exercise.ExerciseId, dto);

        // Assert
        Assert.Same(expectedDto, result);

        Assert.Equal("Updated Name", exercise.Name);
        Assert.Equal("Updated description", exercise.Description);

        Assert.Equal(2, exercise.ExerciseMuscleGroups.Count);

        Assert.Contains(
            exercise.ExerciseMuscleGroups,
            emg => emg.MuscleGroupId == originalPrimaryGroup.MuscleGroupId &&
                   emg.Role == MuscleGroupRole.Secondary
        );

        Assert.Contains(
            exercise.ExerciseMuscleGroups,
            emg => emg.MuscleGroupId == newPrimaryGroup.MuscleGroupId &&
                   emg.Role == MuscleGroupRole.Primary
        );

        Assert.DoesNotContain(
            exercise.ExerciseMuscleGroups,
            emg => emg.MuscleGroupId == originalSecondaryGroup.MuscleGroupId
        );

        exerciseRepositoryMock.Verify(
            repo => repo.UpdateAsync(exercise, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(exercise),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WhenUpdatedExerciseCannotBeRetrieved_ReturnsNull()
    {
        // Arrange
        var exercise = new Exercise("Bench Press", "Chest exercise");
        var muscleGroup = new MuscleGroup("Chest", "Chest muscles");

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

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        exerciseRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                exercise,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        var detailedMapperMock = new Mock<IMapper<Exercise, ExerciseDetailedDto>>();

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(exercise.ExerciseId, dto);

        // Assert
        Assert.Null(result);

        exerciseRepositoryMock.Verify(
            repo => repo.UpdateAsync(exercise, It.IsAny<CancellationToken>()),
            Times.Once
        );

        detailedMapperMock.Verify(
            mapper => mapper.Map(It.IsAny<Exercise>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenExerciseDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Exercise?)null);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.DeleteAsync(exerciseId);

        // Assert
        Assert.False(result);

        exerciseRepositoryMock.Verify(
            repo => repo.DeleteAsync(It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenExerciseExists_ReturnsTrue()
    {
        // Arrange
        var exercise = new Exercise("Bench Press", "Chest exercise");

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.DeleteAsync(
                exercise,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.DeleteAsync(exercise.ExerciseId);

        // Assert
        Assert.True(result);

        exerciseRepositoryMock.Verify(
            repo => repo.DeleteAsync(exercise, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryCannotDeleteExercise_ReturnsFalse()
    {
        // Arrange
        var exercise = new Exercise("Bench Press", "Chest exercise");

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.DeleteAsync(
                exercise,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(exerciseRepositoryMock, muscleGroupRepositoryMock);

        // Act
        var result = await service.DeleteAsync(exercise.ExerciseId);

        // Assert
        Assert.False(result);

        exerciseRepositoryMock.Verify(
            repo => repo.DeleteAsync(exercise, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task UpdateAsync_WhenDescriptionIsNull_UpdatesExerciseWithNullDescription()
    {
        // Arrange
        var exercise = new Exercise("Bench Press", "Old description");
        var muscleGroup = new MuscleGroup("Chest", "Chest muscles");

        var dto = new UpdateExerciseDto
        {
            Name = "Bench Press",
            Description = null,
            MuscleGroups = new()
            {
                new ExerciseMuscleGroupInputDto
                {
                    MuscleGroupId = muscleGroup.MuscleGroupId,
                    Role = MuscleGroupRole.Primary
                }
            }
        };

        var expectedDto = new ExerciseDetailedDto
        {
            ExerciseId = exercise.ExerciseId,
            Name = "Bench Press",
            Description = string.Empty
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdForUpdateAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                exercise.ExerciseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercise);

        exerciseRepositoryMock
            .Setup(repo => repo.UpdateAsync(
                exercise,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();
        muscleGroupRepositoryMock
            .Setup(repo => repo.GetByIdAsync(
                muscleGroup.MuscleGroupId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(muscleGroup);

        var detailedMapperMock = new Mock<IMapper<Exercise, ExerciseDetailedDto>>();
        detailedMapperMock
            .Setup(mapper => mapper.Map(exercise))
            .Returns(expectedDto);

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock,
            detailedMapperMock: detailedMapperMock
        );

        // Act
        var result = await service.UpdateAsync(exercise.ExerciseId, dto);

        // Assert
        Assert.Same(expectedDto, result);
        Assert.Null(exercise.Description);

        exerciseRepositoryMock.Verify(
            repo => repo.UpdateAsync(exercise, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }


    [Fact]
    public async Task UpdateAsync_WhenMuscleGroupsAreNull_ReturnsNull()
    {
        // Arrange
        var exerciseId = Guid.NewGuid();

        var dto = new UpdateExerciseDto
        {
            Name = "Updated Exercise",
            Description = "Updated description",
            MuscleGroups = null!
        };

        var exerciseRepositoryMock = new Mock<IExerciseRepository>();
        var muscleGroupRepositoryMock = new Mock<IMuscleGroupRepository>();

        var service = CreateService(
            exerciseRepositoryMock,
            muscleGroupRepositoryMock
        );

        // Act
        var result = await service.UpdateAsync(exerciseId, dto);

        // Assert
        Assert.Null(result);

        exerciseRepositoryMock.Verify(
            repo => repo.GetByIdForUpdateAsync(
                exerciseId,
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        exerciseRepositoryMock.Verify(
            repo => repo.UpdateAsync(
                It.IsAny<Exercise>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        muscleGroupRepositoryMock.Verify(
            repo => repo.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private static ExerciseService CreateService(
        Mock<IExerciseRepository> exerciseRepositoryMock,
        Mock<IMuscleGroupRepository> muscleGroupRepositoryMock,
        Mock<IMapper<Exercise, ExerciseDetailedDto>>? detailedMapperMock = null,
        Mock<IMapper<Exercise, ExerciseSummaryDto>>? summaryMapperMock = null,
        Mock<IMapper<CreateExerciseDto, Exercise>>? createMapperMock = null)
    {
        detailedMapperMock ??= new Mock<IMapper<Exercise, ExerciseDetailedDto>>();
        summaryMapperMock ??= new Mock<IMapper<Exercise, ExerciseSummaryDto>>();
        createMapperMock ??= new Mock<IMapper<CreateExerciseDto, Exercise>>();

        return new ExerciseService(
            exerciseRepositoryMock.Object,
            muscleGroupRepositoryMock.Object,
            detailedMapperMock.Object,
            summaryMapperMock.Object,
            createMapperMock.Object
        );
    }
}
