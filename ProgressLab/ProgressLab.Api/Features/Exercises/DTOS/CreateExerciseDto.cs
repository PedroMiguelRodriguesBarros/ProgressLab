using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class CreateExerciseDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Muscle group is required.")]
        public string MuscleGroup { get; set; }

    }
}