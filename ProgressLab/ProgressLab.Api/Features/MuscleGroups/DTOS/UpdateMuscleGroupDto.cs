using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class UpdateMuscleGroupDto
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public required string Name { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public required string Description { get; set; }

    }
}