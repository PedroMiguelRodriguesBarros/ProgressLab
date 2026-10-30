using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class UpdateExerciseDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Muscle group is required.")]
        public required Guid MuscleGroupId { get; set; }
    }
}