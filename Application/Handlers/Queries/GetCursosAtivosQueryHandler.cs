using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetCursosAtivosQueryHandler : IRequestHandler<GetCursosAtivosQuery, List<CursoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCursosAtivosQueryHandler> _logger;

        public GetCursosAtivosQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCursosAtivosQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<CursoDto>> Handle(GetCursosAtivosQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consultando cursos ativos");
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var cursos = await _unitOfWork.Cursos.GetCursosAtivosAsync(cancellationToken);
                stopwatch.Stop();

                _logger.LogDebug("Cursos encontrados: {@Cursos}", cursos.Select(c => new { c.Id, c.Nome, c.Ativo }).Take(5));

                return cursos.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar cursos ativos: TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

                throw;
            }
        }

        private CursoDto MapToDto(Curso curso)
        {
            return new CursoDto
            {
                Id = curso.Id,
                Nome = curso.Nome,
                Descricao = curso.Descricao,
                Preco = curso.Preco,
                Duracao = curso.Duracao,
                Ativo = curso.Ativo,
                DataCriacao = curso.DataCriacao
            };
        }

    }
}
