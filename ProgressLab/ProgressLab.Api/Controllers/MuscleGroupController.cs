using Microsoft.AspNetCore.Mvc;

namespace MuscleGroups
{
    [ApiController]
    [Route("api/muscle-groups")]
    public class MuscleGroupController : ControllerBase
    {
        private readonly MuscleGroupService _service;


        public MuscleGroupController(MuscleGroupService muscleGroupService)
        {
            _service = muscleGroupService;
        }

        [HttpPost]
        public async Task<ActionResult<MuscleGroupDetailedDto>> Create([FromBody] CreateMuscleGroupDto dto, CancellationToken ct)
        {
            var muscleGroup = await _service.CreateAsync(dto, ct);
            if (muscleGroup == null)
                return BadRequest("Failed to create muscle group.");

            return CreatedAtAction(nameof(GetById), new { id = muscleGroup.MuscleGroupId}, muscleGroup);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MuscleGroupDetailedDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var muscleGroup = await _service.GetByIdAsync(id, ct);
            if (muscleGroup == null)
                return NotFound();

            return Ok(muscleGroup);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MuscleGroupSummaryDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 100, [FromQuery] CancellationToken ct = default)
        {
            var muscleGroups = await _service.GetAllAsync(page, pageSize, ct);
            return Ok(muscleGroups);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(id, ct);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MuscleGroupDetailedDto>> Update([FromRoute] Guid id, [FromBody] UpdateMuscleGroupDto dto, CancellationToken ct)
        {
            var updatedMuscleGroup = await _service.UpdateAsync(id, dto, ct);
            if (updatedMuscleGroup == null)
                return NotFound();

            return Ok(updatedMuscleGroup);
        }
    }
}