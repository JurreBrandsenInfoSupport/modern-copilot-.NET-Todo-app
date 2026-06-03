using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TodoApp.Application.USR002Users;

namespace TodoApp.API.Controllers
{
    /// <summary>
    /// Manages user registration and retrieval.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    [EnableRateLimiting("fixed")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="command">The user registration details.</param>
        /// <returns>The registered user.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
            => Ok(await _mediator.Send(command));

        /// <summary>
        /// Retrieves all registered users.
        /// </summary>
        /// <returns>A list of all users.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
            => Ok(await _mediator.Send(new GetAllUsersQuery()));
    }
}
