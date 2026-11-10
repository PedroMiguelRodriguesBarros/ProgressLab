using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class CreateMuscleGroupDto
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required]
        [StringLength(2000)]
        public required string Description { get; set; }

    }
}