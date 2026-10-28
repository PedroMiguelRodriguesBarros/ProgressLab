using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class ExerciseDetailedDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Sets are required.")]
        public List<string> Sets { get; set; }

        [Required(ErrorMessage = "Muscle group is required.")]
        public string MuscleGroup { get; set; }

    }
}