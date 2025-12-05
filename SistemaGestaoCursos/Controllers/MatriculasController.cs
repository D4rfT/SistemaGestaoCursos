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
    public class MatriculasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatriculasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<List<MatriculaDto>>> GetAll()
        {
            var matriculas = await _mediator.Send(new GetAllMatriculasQuery());
            return Ok(matriculas);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<MatriculaDto>> GetById(int id)
        {
            var matricula = await _mediator.Send(new GetMatriculaByIdQuery(id));
            return Ok(matricula);
        }

        [HttpGet("minhas-matriculas")]
        [Authorize(Roles = "Aluno")]
        public async Task<ActionResult<List<MatriculaDto>>> GetMinhasMatriculas()
        {
            var alunoId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var matriculas = await _mediator.Send(new GetMatriculasPorAlunoQuery(alunoId));
            return Ok(matriculas);
        }

        [HttpGet("aluno/{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<List<MatriculaDto>>> GetByAlunoId(int id)
        {
            var matriculas = await _mediator.Send(new GetMatriculasPorAlunoQuery(id));
            return Ok(matriculas);
        }

        [HttpGet("curso/{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<List<MatriculaDto>>> GetByCursoId(int id)
        {
            var matriculas = await _mediator.Send(new GetMatriculasPorCursoQuery(id));
            return Ok(matriculas);
        }

        [HttpPost]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult<MatriculaDto>> Create([FromBody] MatricularAlunoCommand command)
        {
            var matricula = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = matricula.Id }, matricula);
        }

        [HttpPatch("ativar/{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult> Ativar(int id)
        {
            var result = await _mediator.Send(new ReativarMatriculaCommand(id));
            return Ok(new { message = "Matrícula ativada com sucesso", success = result });
        }

        [HttpPatch("desativar/{id}")]
        [Authorize(Roles = "Secretaria,Administrador")]
        public async Task<ActionResult> Desativar(int id)
        {
            var result = await _mediator.Send(new DesativarMatriculaCommand(id));
            return Ok(new { message = "Matrícula desativada com sucesso", success = result });
        }

    }
}
