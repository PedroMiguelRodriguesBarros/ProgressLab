namespace Sets
{
    public class SetEntityToSummaryDtoMapper : IMapper<Set, SetSummaryDto>
    {
        public SetSummaryDto Map(Set src)
        {
            return new SetSummaryDto
            {
                SetNumber = src.SetNumber,
                Repetitions = src.Repetitions
            };
        }
    }
}