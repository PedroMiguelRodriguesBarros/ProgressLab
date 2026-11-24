namespace MuscleGroups
{
    public class MuscleGroupService
    {
        private readonly IMuscleGroupRepository _repo;
        private readonly IMapper<MuscleGroup, MuscleGroupDetailedDto> _detailedMapper;
        private readonly IMapper<MuscleGroup, MuscleGroupSummaryDto> _summaryMapper;
        private readonly IMapper<CreateMuscleGroupDto, MuscleGroup> _createMapper;

        public MuscleGroupService(IMuscleGroupRepository repo, IMapper<MuscleGroup, MuscleGroupDetailedDto> detailedMapper, IMapper<MuscleGroup, MuscleGroupSummaryDto> summaryMapper, IMapper<CreateMuscleGroupDto, MuscleGroup> createMapper)
        {
            _repo = repo;
            _detailedMapper = detailedMapper;
            _summaryMapper = summaryMapper;
            _createMapper = createMapper;
        }

        public async Task<MuscleGroupDetailedDto?> GetByIdAsync(Guid muscleGroupId, CancellationToken ct = default)
        {
            var muscleGroup = await _repo.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup == null)
            {
                return null;
            }

            return _detailedMapper.Map(muscleGroup);
        }

        public async Task<IReadOnlyList<MuscleGroupSummaryDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var muscleGroups = await _repo.GetAllAsync(page, pageSize, ct);
            return muscleGroups.Select(m => _summaryMapper.Map(m)).ToList();
        }

        public async Task<MuscleGroupDetailedDto> CreateAsync(CreateMuscleGroupDto dto, CancellationToken ct = default)
        {
            var muscleGroup = _createMapper.Map(dto);
            await _repo.AddAsync(muscleGroup, ct);
            return _detailedMapper.Map(muscleGroup);
        }

        public async Task<MuscleGroupDetailedDto?> UpdateAsync(Guid muscleGroupId, UpdateMuscleGroupDto dto, CancellationToken ct = default)
        {
            var muscleGroup = await _repo.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup == null)
            {
                return null;
            }

            muscleGroup.Name = dto.Name.Trim();
            muscleGroup.Description = dto.Description?.Trim();

            await _repo.UpdateAsync(muscleGroup, ct);

            return _detailedMapper.Map(muscleGroup);
        }

        public async Task<bool> DeleteAsync(Guid muscleGroupId, CancellationToken ct = default)
        {
            var muscleGroup = await _repo.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup == null)
            {
                return false;
            }

            return await _repo.DeleteAsync(muscleGroup, ct);  
        }
    }
}
