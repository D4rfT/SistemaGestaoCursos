using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Handlers.Queries
{
    public class GetAllCursosQueryHandler : IRequestHandler<GetAllCursosQuery, List<CursoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllCursosQueryHandler> _logger;
        private readonly IMemoryCache _cache;

        public GetAllCursosQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllCursosQueryHandler> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<CursoDto>> Handle(GetAllCursosQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "all_cursos";

            if (_cache.TryGetValue(cacheKey, out List<CursoDto> cachedCursos))
            {
                _logger.LogDebug("Retornando cursos do cache");
                return cachedCursos;
            }

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

                var result = cursos.Select(MapToDto).ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                    .SetSlidingExpiration(TimeSpan.FromSeconds(10))
                    .RegisterPostEvictionCallback((key, value, reason, state) =>
                    {
                        _logger.LogDebug($"Cache expirado: {key}, motivo: {reason}");
                    });

                _cache.Set(cacheKey, result, cacheOptions);
                _logger.LogDebug("Cursos salvos no cache por 30 segundos");

                return result;
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
