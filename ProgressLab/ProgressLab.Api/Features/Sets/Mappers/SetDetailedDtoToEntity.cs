namespace Sets
{
    public class SetDetailedDtoToEntityMapper : IMapper<SetDetailedDto, Set>
    {
        public Set Map(SetDetailedDto src)
        {
            return new Set
            {
                SetNumber = src.SetNumber,
                Repetitions = src.Repetitions,
                Weight = src.Weight
            };
        }
    }
}
