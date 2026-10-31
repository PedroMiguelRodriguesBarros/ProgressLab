using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class CreateWorkoutDto
    {
        [Required(ErrorMessage = "Workout name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Workout date is required.")]
        public DateTimeOffset Date { get; set; }

        public string? Notes { get; set; }
    }
}