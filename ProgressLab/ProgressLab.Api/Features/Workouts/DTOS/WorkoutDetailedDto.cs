using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class WorkoutDetailedDto
    {
        [Required(ErrorMessage = "Workout name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Workout date is required.")]
        public DateTimeOffset Date { get; set; }

        [Required(ErrorMessage = "At least one exercise is required.")]
        public List<string> ExerciseNames { get; set; }

        [Required(ErrorMessage = "At least one muscle group is required.")]
        public List<string> MuscleGroupNames { get; set; }

    }
}