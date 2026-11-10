using Sets;
using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class WorkoutExerciseDetailedDto
    {
        public Guid WorkoutExerciseId { get; set; }

        public Guid ExerciseId { get; set; }

        public string ExerciseName { get; set; } = string.Empty;

        public int Order { get; set; }

        public List<SetDetailedDto> Sets { get; set; } = new();
    }
}