using Microsoft.AspNetCore.Mvc;

namespace Workouts
{
    [ApiController]
    [Route("api/workouts")]
    public class WorkoutController : ControllerBase
    {
        private readonly WorkoutService _service;


        public WorkoutController(WorkoutService workoutService)
        {
            _service = workoutService;
        }

        [HttpPost]
        public async Task<ActionResult<WorkoutDetailedDto>> Create([FromBody] CreateWorkoutDto dto, CancellationToken ct)
        {
            var workout = await _service.CreateAsync(dto, ct);
            if (workout == null)
                return BadRequest();

            return Ok(workout);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkoutDetailedDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var workout = await _service.GetByIdAsync(id, ct);
            if (workout == null)
                return NotFound();

            return Ok(workout);
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkoutSummaryDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var workouts = await _service.GetAllAsync(page, pageSize, ct);
            return Ok(workouts);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkoutDetailedDto>> Update([FromRoute] Guid id, [FromBody] UpdateWorkoutDto dto, CancellationToken ct)
        {
            var workout = await _service.UpdateAsync(id, dto, ct);
            if (workout == null)
                return NotFound();

            return Ok(workout);
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