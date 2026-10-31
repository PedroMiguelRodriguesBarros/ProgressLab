using System.ComponentModel.DataAnnotations;

namespace Sets
{
    public class CreateSetDto
    {
        
        [Required(ErrorMessage = "Repetitions is required.")]
        public int Repetitions { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "WorkoutExerciseId is required.")]
        public Guid WorkoutExerciseId { get; set; }

    }
}