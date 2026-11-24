namespace Sets
{
    public class SetEntityToDetailedDtoMapper : IMapper<Set, SetDetailedDto>
    {
        public SetDetailedDto Map(Set src)
        {
            return new SetDetailedDto
            {
                SetId = src.SetId,
                SetNumber = src.SetNumber,
                Repetitions = src.Repetitions,
                Weight = src.Weight
            };
        }
    }
}
