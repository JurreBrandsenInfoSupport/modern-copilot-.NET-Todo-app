using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TodoApp.Application.CMT003Comments;

namespace TodoApp.API.Controllers
{
    /// <summary>
    /// Manages comments on task items.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/comments")]
    [EnableRateLimiting("fixed")]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Adds a comment to a task item.
        /// </summary>
        /// <param name="command">The comment details including task item ID and content.</param>
        /// <returns>The newly created comment.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddComment([FromBody] AddCommentCommand command)
        {
            var comment = await _mediator.Send(command);
            return Ok(comment);
        }

        /// <summary>
        /// Retrieves all comments for a specific task item.
        /// </summary>
        /// <param name="taskItemId">The ID of the task item to retrieve comments for.</param>
        /// <returns>A list of comments for the specified task.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetComments([FromQuery] int taskItemId)
        {
            var comments = await _mediator.Send(new GetCommentsQuery(taskItemId));
            return Ok(comments);
        }
    }
}
