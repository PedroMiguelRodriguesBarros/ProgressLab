using System.ComponentModel.DataAnnotations;

namespace Sets
{
    public class SetDetailedDto
    {               

        public Guid SetId { get; set; }

        public int SetNumber { get; set; }
        
        public int Repetitions { get; set; }

        public decimal Weight { get; set; }

    }
}