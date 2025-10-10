using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.TSK001Tasks;
using TodoApp.Application.TSK002TaskDueDates;

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
        public async Task<IActionResult> GetAll([FromQuery] int? userId, [FromQuery] bool? overdueOnly)
        {
            if (overdueOnly.HasValue && overdueOnly.Value)
            {
                var overdueResult = await _mediator.Send(new GetTasksWithDueDatesQuery(userId, true));
                return Ok(overdueResult);
            }
            else if (userId.HasValue)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                var command = new UpdateTaskCommand(id, request.Title, request.DueDate, request.IsCompleted);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public record UpdateTaskRequest(string? Title, DateTime? DueDate, bool? IsCompleted);
}
