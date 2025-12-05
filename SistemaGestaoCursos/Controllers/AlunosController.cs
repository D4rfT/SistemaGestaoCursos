using Application.Commands;
using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaGestaoCursos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AlunosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlunosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<List<AlunoDto>>> GetAll()
        {
            var alunos = await _mediator.Send(new GetAllAlunosQuery());
            return Ok(alunos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<AlunoDto>> GetById(int id)
        {
            var aluno = await _mediator.Send(new GetAlunoByIdQuery(id));
            return Ok(aluno);
        }

        [HttpGet("meus-dados")]
        [Authorize(Roles = "Aluno")]
        public async Task<ActionResult<AlunoDto>> GetMeusDados()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var aluno = await _mediator.Send(new GetAlunoByUsuarioIdQuery(usuarioId));
            return Ok(aluno);
        }

        [HttpPost]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<AlunoDto>> Create([FromBody] CreateAlunoCommand command)
        {
            var aluno = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = aluno.Id }, aluno);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<AlunoDto>> Update(int id, [FromBody] UpdateAlunoCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID inserido não compatível com o ID encontrado");

            var aluno = await _mediator.Send(command);
            return Ok(aluno);
        }

        [HttpPatch("ativar/{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult> Ativar(int id)
        {
            var result = await _mediator.Send(new ReativarAlunoCommand(id));
            return Ok(new { message = "Aluno ativado com sucesso", success = result });
        }

        [HttpPatch("desativar/{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult> Desativar(int id)
        {
            var result = await _mediator.Send(new DesativarAlunoCommand(id));
            return Ok(new { message = "Aluno desativado com sucesso", success = result });
        }


    }
}
