using Application.Commands;
using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestaoCursos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlunosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AlunoDto>>> GetAll()
        {
            var alunos = await _mediator.Send(new GetAllAlunosQuery());
            return Ok(alunos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlunoDto>> GetById(int id)
        {
            var aluno = await _mediator.Send(new GetAlunoByIdQuery(id));
            return Ok(aluno);
        }

        [HttpPost]
        public async Task<ActionResult<AlunoDto>> Create([FromBody] CreateAlunoCommand command)
        {
            var aluno = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = aluno.Id }, aluno);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AlunoDto>> Update(int id, [FromBody] UpdateAlunoCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID inserido não compatível com o ID encontrado");

            var aluno = await _mediator.Send(command);
            return Ok(aluno);
        }

        [HttpPatch("ativar/{id}")]
        public async Task<ActionResult> Ativar(int id)
        {
            var result = await _mediator.Send(new ReativarAlunoCommand(id));
            return Ok(new { message = "Aluno ativado com sucesso", success = result });
        }

        [HttpPatch("desativar/{id}")]
        public async Task<ActionResult> Desativar(int id)
        {
            var result = await _mediator.Send(new DesativarAlunoCommand(id));
            return Ok(new { message = "Aluno desativado com sucesso", success = result });
        }
    }
}
