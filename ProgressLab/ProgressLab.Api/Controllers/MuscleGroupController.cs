using Microsoft.AspNetCore.Mvc;

namespace MuscleGroups
{
    [ApiController]
    [Route("api/muscle-groups")]
    public class MuscleGroupController : ControllerBase
    {
        private readonly MuscleGroupService _service;
        private readonly IMapper<MuscleGroup, MuscleGroupDetailedDto> _mapper;


        public MuscleGroupController(MuscleGroupService muscleGroupService, IMapper<MuscleGroup, MuscleGroupDetailedDto> mapper)
        {
            _service = muscleGroupService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MuscleGroupDetailedDto>> Create([FromBody] CreateMuscleGroupDto dto, CancellationToken ct)
        {
            var muscleGroup = await _service.CreateAsync(dto, ct);
            if (muscleGroup == null)
                return BadRequest("Failed to create muscle group.");

            return CreatedAtAction(nameof(GetById), new { id = muscleGroup.MuscleGroupId });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MuscleGroupDetailedDto>> GetById([FromRoute] Guid muscleGroupId, CancellationToken ct)
        {
            var muscleGroup = await _service.GetByIdAsync(muscleGroupId, ct);
            if (muscleGroup == null)
                return NotFound();

            return Ok(muscleGroup);
        }

        [HttpGet]
        public async Task<ActionResult<List<MuscleGroupDetailedDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var muscleGroups = await _service.GetAllAsync(page, pageSize, ct);
            return Ok(muscleGroups);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid muscleGroupId, CancellationToken ct)
        {
            await _service.DeleteAsync(muscleGroupId, ct);
            return Ok("Muscle group deleted successfully.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MuscleGroupDetailedDto>> Update([FromRoute] Guid muscleGroupId, [FromBody] UpdateMuscleGroupDto dto, CancellationToken ct)
        {
            var updatedMuscleGroup = await _service.UpdateAsync(muscleGroupId, dto, ct);
            if (updatedMuscleGroup == null)
                return NotFound();

            return Ok(updatedMuscleGroup);
        }
    }
}