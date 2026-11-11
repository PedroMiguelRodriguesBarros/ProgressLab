using System.ComponentModel.DataAnnotations;

namespace Workouts
{
    public class UpdateWorkoutDto
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public DateTimeOffset? Date { get; set; }
    
        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}