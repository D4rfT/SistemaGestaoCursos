using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetCursosComFiltrosQueryHandler : IRequestHandler<GetCursosComFiltrosQuery, List<CursoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCursosComFiltrosQueryHandler> _logger;

        public GetCursosComFiltrosQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCursosComFiltrosQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<CursoDto>> Handle(GetCursosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Consultando cursos com filtros: Nome={request.Nome}, Ativo={request.Ativo}, OrdenarPor={request.OrdenarPor}");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var cursos = await _unitOfWork.Cursos.GetCursosComFiltrosAsync(
                    nome: request.Nome,
                    precoMinimo: request.PrecoMinimo,
                    precoMaximo: request.PrecoMaximo,
                    duracaoMinima: request.DuracaoMinima,
                    duracaoMaxima: request.DuracaoMaxima,
                    ativo: request.Ativo,
                    ordenarPor: request.OrdenarPor,
                    ordemDescendente: request.OrdemDescendente,
                    cancellationToken: cancellationToken);

                stopwatch.Stop();

                _logger.LogInformation($"Consulta com filtros concluída: Total={cursos.Count()}, Tempo={stopwatch.ElapsedMilliseconds}ms");

                if (cursos.Count() == 0)
                {
                    _logger.LogDebug("Nenhum curso encontrado com os filtros aplicados");
                }

                return cursos.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro na consulta com filtros: Tempo={stopwatch.ElapsedMilliseconds}ms");
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