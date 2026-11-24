using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class UpdateExerciseDto
    {

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Name { get; set; }

        [StringLength(2000, MinimumLength = 1)]
        public string? Description { get; set; }

        [Required]
        [MinLength(1)]
        public List<ExerciseMuscleGroupInputDto>? MuscleGroups { get; set; } = new();
    }
}