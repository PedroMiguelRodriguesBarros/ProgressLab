using System.ComponentModel.DataAnnotations;

namespace MuscleGroups
{
    public class MuscleGroupSummaryDto
    {
        public Guid MuscleGroupId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}