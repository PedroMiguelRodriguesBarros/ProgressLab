using Microsoft.AspNetCore.Mvc;

namespace Exercises
{
    [ApiController]
    [Route("api/exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly ExerciseService _service;


        public ExerciseController(ExerciseService exerciseService)
        {
            _service = exerciseService;
        }

        [HttpPost]
        public async Task<ActionResult<ExerciseDetailedDto>> Create([FromBody] CreateExerciseDto dto, CancellationToken ct)
        {
            var exercise = await _service.CreateAsync(dto, ct);
            if (exercise == null)
                return BadRequest("Failed to create exercise.");

            return CreatedAtAction(nameof(GetById), new { id = exercise.ExerciseId }, exercise);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ExerciseDetailedDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var exercise = await _service.GetByIdAsync(id, ct);
            if (exercise == null)
                return NotFound();

            return Ok(exercise);
        }

        [HttpGet]
        public async Task<ActionResult<List<ExerciseSummaryDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var exercises = await _service.GetAllAsync(page, pageSize, ct);
            return Ok(exercises);
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
        public async Task<ActionResult<ExerciseDetailedDto>> Update([FromRoute] Guid id, [FromBody] UpdateExerciseDto dto, CancellationToken ct)
        {
            var updatedExercise = await _service.UpdateAsync(id, dto, ct);
            if (updatedExercise == null)
                return NotFound();

            return Ok(updatedExercise);
        }
    }
}