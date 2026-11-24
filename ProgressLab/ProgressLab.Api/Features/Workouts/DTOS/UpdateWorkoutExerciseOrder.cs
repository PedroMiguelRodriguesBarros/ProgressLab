using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class UpdateWorkoutExerciseOrderDto
    {
        [Range(1, int.MaxValue)]
        public int NewOrder { get; set; }
    }
}