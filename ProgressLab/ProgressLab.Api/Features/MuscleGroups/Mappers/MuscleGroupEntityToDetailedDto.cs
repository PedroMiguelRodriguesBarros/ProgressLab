namespace MuscleGroups
{
    public class MuscleGroupEntityToDetailedDtoMapper : IMapper<MuscleGroup, MuscleGroupDetailedDto>
    {
        public MuscleGroupDetailedDto Map(MuscleGroup src)
        {
            return new MuscleGroupDetailedDto
            {
                Name = src.Name,
                Description = src.Description,
            };
        }
    }
}
