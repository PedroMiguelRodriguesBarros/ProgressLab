using System.ComponentModel.DataAnnotations;

namespace Exercises
{
    public class UpdateExerciseDto
    {
        [Required(ErrorMessage = "Duration is required.")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Sets are required.")]
        public List<SetDetailedDto> Sets { get; set; }
    }
}