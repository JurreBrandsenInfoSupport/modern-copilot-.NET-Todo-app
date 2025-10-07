using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.BAL004BalloonAnimals;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/balloon-animals")]
    public class BalloonAnimalsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BalloonAnimalsController(IMediator mediator) => _mediator = mediator;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateBalloonAnimalCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllBalloonAnimalsQuery());
            return Ok(result);
        }
    }
}