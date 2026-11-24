namespace MuscleGroups
{
    public class MuscleGroupEntityToDetailedDtoMapper : IMapper<MuscleGroup, MuscleGroupDetailedDto>
    {
        public MuscleGroupDetailedDto Map(MuscleGroup src)
        {
            return new MuscleGroupDetailedDto
            {
                MuscleGroupId = src.MuscleGroupId,
                Name = src.Name.Trim(),
                Description = src.Description?.Trim(),
            };
        }
    }
}
