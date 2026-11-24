using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class UpdateMuscleGroupDto
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Name { get; set; }

        [StringLength(2000, MinimumLength = 1)]
        public string? Description { get; set; }

    }
}