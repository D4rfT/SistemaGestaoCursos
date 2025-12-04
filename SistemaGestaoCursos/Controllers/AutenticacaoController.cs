using Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestaoCursos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AutenticacaoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new AutenticarCommand(request.Email, request.Senha);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}