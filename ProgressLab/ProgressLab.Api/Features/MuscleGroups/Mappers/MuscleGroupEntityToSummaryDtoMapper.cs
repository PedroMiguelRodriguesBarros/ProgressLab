namespace MuscleGroups
{
    public class MuscleGroupEntityToSummaryDtoMapper : IMapper<MuscleGroup, MuscleGroupSummaryDto>
    {
        public MuscleGroupSummaryDto Map(MuscleGroup src)
        {
            return new MuscleGroupSummaryDto
            {
                MuscleGroupId = src.MuscleGroupId,
                Name = src.Name
            };
        }
    }
}