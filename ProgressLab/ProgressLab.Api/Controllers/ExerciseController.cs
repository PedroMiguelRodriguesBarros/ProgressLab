using Microsoft.AspNetCore.Mvc;

namespace Exercises
{
    [ApiController]
    [Route("api/exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly ExerciseService _service;
        private readonly IMapper<Exercise, ExerciseDetailedDto> _mapper;


        public ExerciseController(ExerciseService exerciseService, IMapper<Exercise, ExerciseDetailedDto> mapper)
        {
            _service = exerciseService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ExerciseDetailedDto>> Create([FromBody] CreateExerciseDto dto, CancellationToken ct)
        {
            var exercise = await _service.CreateAsync(dto, ct);
            if (exercise == null)
                return BadRequest("Failed to create exercise.");

            return CreatedAtAction(nameof(GetById), new { id = exercise.ExerciseId }, _mapper.Map(exercise));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseDetailedDto>> GetById([FromRoute] Guid exerciseId, CancellationToken ct)
        {
            var exercise = await _service.GetByIdAsync(exerciseId, ct);
            if (exercise == null)
                return NotFound();

            return Ok(_mapper.Map(exercise));
        }

        [HttpGet]
        public async Task<ActionResult<List<ExerciseDetailedDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var exercises = await _service.GetAllAsync(page, pageSize, ct);
            var exerciseDtos = exercises.Select(e => _mapper.Map(e));
            return Ok(exerciseDtos);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid exerciseId, CancellationToken ct)
        {
            await _service.DeleteAsync(exerciseId, ct);
            return Ok("Exercise deleted successfully.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ExerciseDetailedDto>> Update([FromRoute] Guid exerciseId, [FromBody] UpdateExerciseDto dto, CancellationToken ct)
        {
            var updatedExercise = await _service.UpdateExerciseAsync(exerciseId, dto, ct);
            if (updatedExercise == null)
                return NotFound();

            return Ok(_mapper.Map(updatedExercise));
        }
    }
}