using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TodoApp.Application.CMT003Comments;

namespace TodoApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/comments")]
    [EnableRateLimiting("fixed")]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
        {
            var comment = await _mediator.Send(command);
            return Ok(comment);
        }

        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] int taskItemId)
        {
            var comments = await _mediator.Send(new GetCommentsQuery(taskItemId));
            return Ok(comments);
        }
    }
}
