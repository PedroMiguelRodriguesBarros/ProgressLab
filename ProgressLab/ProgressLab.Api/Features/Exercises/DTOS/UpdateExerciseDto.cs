using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class UpdateExerciseDto
    {

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Name { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string Description { get; set; }

        [Required]
        [MinLength(1)]
        public List<ExerciseMuscleGroupInputDto>? MuscleGroups { get; set; } = new();
    }
}