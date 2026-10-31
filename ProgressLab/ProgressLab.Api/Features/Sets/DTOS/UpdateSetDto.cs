using System.ComponentModel.DataAnnotations;

namespace Sets
{
    public class UpdateSetDto
    {
        [Required(ErrorMessage = "Repetitions is required.")]
        public int Repetitions { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        public decimal Weight { get; set; }

    }
}