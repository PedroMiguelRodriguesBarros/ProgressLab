using System.ComponentModel.DataAnnotations;

namespace Sets
{
    public class CreateSetDto
    {
        
        [Range(1, int.MaxValue)]
        public int Repetitions { get; set; }

        [Range(typeof(decimal), "0", "9999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
        public decimal Weight { get; set; }
    }
}