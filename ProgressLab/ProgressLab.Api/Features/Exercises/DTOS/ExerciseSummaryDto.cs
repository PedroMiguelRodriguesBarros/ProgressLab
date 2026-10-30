using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class ExerciseSummaryDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Muscle group is required.")]
        public required string MuscleGroup { get; set; }

    }
}