using Application.Commands;
using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestaoCursos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatriculasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MatriculaDto>>> GetAll()
        {
            var matriculas = await _mediator.Send(new GetAllMatriculasQuery());
            return Ok(matriculas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatriculaDto>> GetById(int id)
        {
            var matricula = await _mediator.Send(new GetMatriculaByIdQuery(id));
            return Ok(matricula);
        }

        [HttpGet("aluno/{id}")]
        public async Task<ActionResult<List<MatriculaDto>>> GetByAlunoId(int id)
        {
            var matriculas = await _mediator.Send(new GetMatriculasPorAlunoQuery(id));
            return Ok(matriculas);
        }

        [HttpGet("curso/{id}")]
        public async Task<ActionResult<List<MatriculaDto>>> GetByCursoId(int id)
        {
            var matriculas = await _mediator.Send(new GetMatriculasPorCursoQuery(id));
            return Ok(matriculas);
        }

        [HttpPost]
        public async Task<ActionResult<MatriculaDto>> Create([FromBody] MatricularAlunoCommand command)
        {
            var matricula = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = matricula.Id }, matricula);
        }

        [HttpPatch("ativar/{id}")]
        public async Task<ActionResult> Ativar(int id)
        {
            var result = await _mediator.Send(new ReativarMatriculaCommand(id));
            return Ok(new { message = "Matrícula ativada com sucesso", success = result });
        }

        [HttpPatch("desativar/{id}")]
        public async Task<ActionResult> Desativar(int id)
        {
            var result = await _mediator.Send(new DesativarMatriculaCommand(id));
            return Ok(new { message = "Matrícula desativada com sucesso", success = result });
        }
    }
}
