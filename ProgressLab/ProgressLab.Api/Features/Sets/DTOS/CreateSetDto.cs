using System.ComponentModel.DataAnnotations;

namespace Sets
{
    public class CreateSetDto
    {
        [Required(ErrorMessage = "Set number is required.")]               
        public int SetNumber { get; set; }
        
        [Required(ErrorMessage = "Repetitions is required.")]
        public int Repetitions { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public float Weight { get; set; }

    }
}