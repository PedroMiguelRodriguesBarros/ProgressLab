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

            return CreatedAtAction(nameof(GetById), new { id = workout.WorkoutId }, workout);
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

        [HttpPost("{workoutId:guid}/exercises/{exerciseId:guid}")]
        public async Task<IActionResult> AddExercise([FromRoute] Guid workoutId, [FromRoute] Guid exerciseId, CancellationToken ct)
        {
            try
                {
                    await _service.AddExerciseAsync(workoutId, exerciseId, ct);
                    return NoContent();
                }
                catch (ArgumentException)
                {
                    return NotFound();
                }
                catch (InvalidOperationException)
                {
                    return Conflict();
                }
        }

        [HttpPut("{workoutId:guid}/exercises/{exerciseId:guid}/order")]
        public async Task<IActionResult> UpdateExerciseOrder([FromRoute] Guid workoutId, [FromRoute] Guid exerciseId, [FromBody] UpdateWorkoutExerciseOrderDto dto, CancellationToken ct)
        {
            try
            {
                await _service.UpdateExerciseOrderAsync(workoutId, exerciseId, dto.NewOrder, ct);
                return NoContent();
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{workoutId:guid}/exercises/{exerciseId:guid}")] 
        public async Task<IActionResult> RemoveExercise([FromRoute] Guid workoutId, [FromRoute] Guid exerciseId, CancellationToken ct)
        {
            try
            {
                await _service.RemoveExerciseAsync(workoutId, exerciseId, ct);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}


