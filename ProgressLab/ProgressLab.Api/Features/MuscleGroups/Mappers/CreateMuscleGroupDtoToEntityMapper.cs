namespace MuscleGroups
{
    public class CreateMuscleGroupDtoToEntityMapper : IMapper<CreateMuscleGroupDto, MuscleGroup>
    {
        public MuscleGroup Map(CreateMuscleGroupDto src)
        {
            return new MuscleGroup(
                src.Name.Trim(),
                src.Description?.Trim()
            );
        }
    }
}