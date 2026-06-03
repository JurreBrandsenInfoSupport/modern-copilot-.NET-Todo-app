using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Application.USR002Users;

namespace TodoApp.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
            => Ok(await _mediator.Send(command));

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _mediator.Send(new GetAllUsersQuery()));
    }
}
