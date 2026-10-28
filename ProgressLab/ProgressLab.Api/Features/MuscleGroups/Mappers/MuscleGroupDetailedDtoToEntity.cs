namespace MuscleGroups
{
    public class MuscleGroupDetailedDtoToEntityMapper : IMapper<MuscleGroupDetailedDto, MuscleGroup>
    {
        public MuscleGroup Map(MuscleGroupDetailedDto src)
        {
            return new MuscleGroup
            {
                Name = src.Name,
                Description = src.Description,
            };
        }
    }
}
