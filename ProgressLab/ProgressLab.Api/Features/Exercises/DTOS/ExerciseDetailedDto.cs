using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class ExerciseDetailedDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Muscle group is required.")]
        public required string MuscleGroup { get; set; }

    }
}