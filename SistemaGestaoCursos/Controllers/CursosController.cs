using Application.Commands;
using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestaoCursos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CursosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<CursoDto>>> GetAll()
        {
            var cursos = await _mediator.Send(new GetAllCursosQuery());
            return Ok(cursos);
        }

        [HttpGet("ativos")]
        public async Task<ActionResult<List<CursoDto>>> GetAtivos()
        {
            var cursos = await _mediator.Send(new GetCursosAtivosQuery());
            return Ok(cursos);
        }

        [HttpGet("filtros")]
        public async Task<ActionResult<List<CursoDto>>> GetComFiltros(
            [FromQuery] string? nome = null,
            [FromQuery] decimal? precoMinimo = null,
            [FromQuery] decimal? precoMaximo = null,
            [FromQuery] int? duracaoMinima = null,
            [FromQuery] int? duracaoMaxima = null,
            [FromQuery] bool? ativo = null,
            [FromQuery] string? ordenarPor = null,
            [FromQuery] bool ordemDescendente = false)
        {
            var query = new GetCursosComFiltrosQuery(
                nome, precoMinimo, precoMaximo, duracaoMinima,
                duracaoMaxima, ativo, ordenarPor, ordemDescendente);

            var cursos = await _mediator.Send(query);
            return Ok(cursos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> GetById(int id)
        {
            var curso = await _mediator.Send(new GetCursoByIdQuery(id));
            return Ok(curso);
        }

        [HttpPost]
        public async Task<ActionResult<CursoDto>> Create([FromBody] CreateCursoCommand command)
        {
            var curso = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = curso.Id }, curso);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CursoDto>> Update(int id, [FromBody] UpdateCursoCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID inserido não compatível com o ID encontrado");

            var curso = await _mediator.Send(command);
            return Ok(curso);
        }

        [HttpPatch("ativar/{id}")]
        public async Task<ActionResult> Ativar(int id)
        {
            var result = await _mediator.Send(new ReativarCursoCommand(id));
            return Ok(new { message = "Curso ativado com sucesso", success = result });
        }

        [HttpPatch("desativar/{id}")]
        public async Task<ActionResult> Desativar(int id)
        {
            var result = await _mediator.Send(new DesativarCursoCommand(id));
            return Ok(new { message = "Curso desativado com sucesso", success = result });
        }
    }
}