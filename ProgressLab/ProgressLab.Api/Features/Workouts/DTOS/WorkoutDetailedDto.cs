using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class WorkoutDetailedDto
    {
        public Guid WorkoutId { get; set; }
        public string Name { get; set; } = string.Empty;

        public DateTimeOffset Date { get; set; }

        public string? Notes { get; set; }
        public List<WorkoutExerciseDetailedDto> WorkoutExercises { get; set; } = new();


    }
}