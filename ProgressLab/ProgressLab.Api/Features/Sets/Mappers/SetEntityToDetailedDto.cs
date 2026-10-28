namespace Sets
{
    public class SetEntityToDetailedDtoMapper : IMapper<Set, SetDetailedDto>
    {
        public SetDetailedDto Map(Set src)
        {
            return new SetDetailedDto
            {
                SetNumber = src.SetNumber,
                Repetitions = src.Repetitions,
                Weight = src.Weight
            };
        }
    }
}
