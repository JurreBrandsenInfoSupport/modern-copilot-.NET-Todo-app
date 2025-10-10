using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.RMD001Reminders;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [Route("api/reminders")]
    public class RemindersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RemindersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReminderCommand command)
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

        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _mediator.Send(new GetPendingRemindersQuery());
            return Ok(result);
        }

        [HttpGet("task/{taskItemId}")]
        public async Task<IActionResult> GetByTask(int taskItemId)
        {
            var result = await _mediator.Send(new GetRemindersByTaskQuery(taskItemId));
            return Ok(result);
        }

        [HttpPut("{id}/mark-sent")]
        public async Task<IActionResult> MarkSent(int id)
        {
            try
            {
                var result = await _mediator.Send(new MarkReminderSentCommand(id));
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}