using Microsoft.AspNetCore.Mvc;

namespace Workouts
{
    [ApiController]
    [Route("api/workouts")]
    public class WorkoutController : ControllerBase
    {
        private readonly WorkoutService _service;
        private readonly IMapper<Workout, WorkoutDetailedDto> _mapper;


        public WorkoutController(WorkoutService workoutService, IMapper<Workout, WorkoutDetailedDto> mapper)
        {
            _service = workoutService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<WorkoutDetailedDto>> Create([FromBody] CreateWorkoutDto dto, CancellationToken ct)
        {
            var workout = await _service.CreateAsync(dto, ct);
            if (workout == null)
                return BadRequest();

            return Ok(_mapper.Map(workout));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WorkoutDetailedDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var workout = await _service.GetByIdAsync(id, ct);
            if (workout == null)
                return NotFound();

            return Ok(_mapper.Map(workout));
        }

        [HttpGet]
        public async Task<ActionResult<List<WorkoutDetailedDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var workouts = await _service.GetAllAsync(page, pageSize, ct);
            var workoutDtos = workouts.Select(w => _mapper.Map(w));
            return Ok(workoutDtos);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WorkoutDetailedDto>> Update([FromRoute] Guid id, [FromBody] UpdateWorkoutDto dto, CancellationToken ct)
        {
            var workout = await _service.UpdateWorkoutAsync(id, dto, ct);
            if (workout == null)
                return NotFound();

            return Ok(_mapper.Map(workout));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid workoutId, CancellationToken ct)
        {
            var workout = await _service.GetByIdAsync(workoutId, ct);
            if (workout == null)
                return NotFound();

            await _service.DeleteAsync(workoutId, ct);
            return Ok();
        }
    }
}