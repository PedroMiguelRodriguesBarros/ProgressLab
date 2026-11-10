using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class WorkoutSummaryDto
    {
        public Guid WorkoutId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
    }
}