using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.TSK001Tasks;

namespace TodoApp.API.Controllers
{
    /// <summary>
    /// Manages task items in the Todo application.
    /// </summary>
    [ApiController]
    [Route("api/tasks")]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TasksController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="command">The task creation details.</param>
        /// <returns>The newly created task.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Retrieves all tasks, optionally filtered by user ID.
        /// </summary>
        /// <param name="userId">Optional user ID to filter tasks by assignee.</param>
        /// <returns>A list of tasks.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
