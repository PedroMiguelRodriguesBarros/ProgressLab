using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class CreateMuscleGroupDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

    }
}