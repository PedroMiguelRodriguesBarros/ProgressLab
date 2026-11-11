namespace Exercises
{
    public class CreateExerciseDtoToEntityMapper : IMapper<CreateExerciseDto, Exercise>
    {
        public Exercise Map(CreateExerciseDto src)
        {
            return new Exercise(
                src.Name.Trim(),
                src.Description.Trim()
            );
        }
    }
}