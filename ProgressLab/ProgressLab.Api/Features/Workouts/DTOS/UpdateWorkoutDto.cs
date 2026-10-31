using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class UpdateWorkoutDto
    {
        [Required(ErrorMessage = "Workout name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Workout date is required.")]
        public DateTimeOffset Date { get; set; }

        public string? Notes { get; set; }

        [Required(ErrorMessage = "At least one exercise is required.")]
        public List<Guid> Exercises { get; set; }
    }
}