using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class MuscleGroupDetailedDto
    {
        public Guid MuscleGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}