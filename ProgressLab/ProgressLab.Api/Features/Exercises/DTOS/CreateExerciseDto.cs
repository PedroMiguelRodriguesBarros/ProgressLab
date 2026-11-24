using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class CreateExerciseDto
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MinLength(1)]
        public List<ExerciseMuscleGroupInputDto> MuscleGroups { get; set; } = new();
    }
}