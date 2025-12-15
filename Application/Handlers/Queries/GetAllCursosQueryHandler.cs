using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetAllCursosQueryHandler : IRequestHandler<GetAllCursosQuery, List<CursoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCursosQueryHandler> _logger;

        public GetAllCursosQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCursosQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<CursoDto>> Handle(GetAllCursosQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Iniciando consulta de todos os cursos");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var cursos = await _unitOfWork.Cursos.GetAllAsync(cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation($"Consulta de cursos concluída: TotalCursos={cursos.Count()}, TempoExecucao={stopwatch.ElapsedMilliseconds}ms");

                if (cursos.Count() == 0)
                {
                    _logger.LogWarning("Consulta de cursos retornou 0 resultados");
                }
                else
                {
                    _logger.LogDebug("Cursos encontrados: {@Cursos}", cursos.Select(c => new { c.Id, c.Nome, c.Ativo }).Take(5));

                    var cursosAtivos = cursos.Count(c => c.Ativo);
                    var cursosInativos = cursos.Count(c => !c.Ativo);

                    _logger.LogDebug($"Estatísticas: Ativos={cursosAtivos}, Inativos={cursosInativos}");
                }

                return cursos.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar cursos: TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

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
