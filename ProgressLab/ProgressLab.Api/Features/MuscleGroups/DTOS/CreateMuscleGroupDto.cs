using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class CreateMuscleGroupDto
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

    }
}