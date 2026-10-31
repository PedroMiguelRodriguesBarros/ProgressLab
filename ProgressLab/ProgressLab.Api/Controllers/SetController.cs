using Microsoft.AspNetCore.Mvc;

namespace Sets
{
    [ApiController]
    [Route("api/sets")]
    public class SetController : ControllerBase
    {
        private readonly SetService _service;
        private readonly IMapper<Set, SetDetailedDto> _mapper;


        public SetController(SetService setService, IMapper<Set, SetDetailedDto> mapper)
        {
            _service = setService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<SetDetailedDto>> Create([FromBody] CreateSetDto dto, CancellationToken ct)
        {
            var set = await _service.CreateAsync(dto, ct);
            if (set == null)
                return NotFound();

            return Ok(_mapper.Map(set));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SetDetailedDto>> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var set = await _service.GetByIdAsync(id, ct);
            if (set == null)
                return NotFound();

            return Ok(_mapper.Map(set));
        }

        [HttpGet]
        public async Task<ActionResult<List<SetDetailedDto>>> GetAll(int page = 1, int pageSize = 100, CancellationToken ct = default)
        {
            var sets = await _service.GetAllAsync(page, pageSize, ct);
            var setDtos = sets.Select(s => _mapper.Map(s));
            return Ok(setDtos);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SetDetailedDto>> Update([FromRoute] Guid id, [FromBody] UpdateSetDto dto, CancellationToken ct)
        {
            var updatedSet = await _service.UpdateSetAsync(id, dto, ct);
            if (updatedSet == null)
                return NotFound();

            return Ok(_mapper.Map(updatedSet));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid setId, CancellationToken ct)
        {
            await _service.DeleteAsync(setId, ct);
            return Ok("Set deleted successfully.");
        }
    }
}