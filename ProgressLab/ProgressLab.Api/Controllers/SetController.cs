using Microsoft.AspNetCore.Mvc;

namespace Sets
{
    [ApiController]
    [Route("api/sets")]
    public class SetController : ControllerBase
    {
        private readonly SetService _service;

        public SetController(SetService setService)
        {
            _service = setService;
        }

        [HttpPost]
        [Route("~/api/workout-exercises/{workoutExerciseId:guid}/sets")]
        public async Task<ActionResult<SetDetailedDto>> Create([FromRoute] Guid workoutExerciseId, [FromBody] CreateSetDto dto, CancellationToken ct)
        {
            try
            {
                var set = await _service.CreateAsync(workoutExerciseId, dto, ct);

                return CreatedAtAction(nameof(GetById), new { id = set.SetId }, set);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SetDetailedDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            try
            {
                var set = await _service.GetByIdAsync(id, ct);
                return Ok(set);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("~/api/workout-exercises/{workoutExerciseId:guid}/sets")]
        public async Task<ActionResult<IReadOnlyList<SetSummaryDto>>> GetAllByWorkoutExerciseId([FromRoute] Guid workoutExerciseId, CancellationToken ct)
        {
            try
            {
                var sets = await _service.GetAllByWorkoutExerciseIdAsync(workoutExerciseId, ct);
                return Ok(sets);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<SetSummaryDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var sets = await _service.GetAllAsync(page, pageSize, ct);
            return Ok(sets);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<SetDetailedDto>> Update([FromRoute] Guid id, [FromBody] UpdateSetDto dto, CancellationToken ct)
        {
            var updatedSet = await _service.UpdateSetAsync(id, dto, ct);
            if (updatedSet == null)
                return NotFound();

            return Ok(updatedSet);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var deleted = await _service.DeleteAsync(id, ct);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}