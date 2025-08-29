using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.TSK001Tasks;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TasksController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
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
        public async Task<IActionResult> GetAll([FromQuery] int? userId)
        {
            if (userId.HasValue)
            {
                var result = await _mediator.Send(new GetTasksByUserQuery(userId.Value));
                return Ok(result);
            }
            else
            {
                var result = await _mediator.Send(new GetAllTasksQuery());
                return Ok(result);
            }
        }
    }
}
