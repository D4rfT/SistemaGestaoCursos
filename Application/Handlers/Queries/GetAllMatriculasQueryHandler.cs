using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetAllMatriculasQueryHandler : IRequestHandler<GetAllMatriculasQuery, List<MatriculaDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllMatriculasQueryHandler> _logger;

        public GetAllMatriculasQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllMatriculasQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<MatriculaDto>> Handle(GetAllMatriculasQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Iniciando consulta de todos as matrículas");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var matriculas = await _unitOfWork.Matriculas.GetMatriculasComDadosRelacionadosAsync(cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation($"Consulta de matriculas concluída: TotalAlunos={matriculas.Count()}, TempoExecucao={stopwatch.ElapsedMilliseconds}ms");

                if(matriculas.Count() == 0)
                {
                    _logger.LogWarning("Consulta de matrículas retornou 0 resultados");
                }
                else
                {
                    _logger.LogDebug("Matrículas encontrados: {@Matriculas}", matriculas.Select(m => new {m.Id, m.AlunoId, m.CursoId, m.DataMatricula, m.Ativa}).Take(5));

                    var matriculasAtivas = matriculas.Count(m => m.Ativa);
                    var matriculasInativas = matriculas.Count(m => !m.Ativa);

                    _logger.LogDebug($"Estatísticas: Ativas={matriculasAtivas}, Inativas={matriculasInativas}");
                }

                    return matriculas.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar Matrículas: TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

                throw;
            }
        }

        private MatriculaDto MapToDto(Matricula matricula)
        {
            return new MatriculaDto
            {
                Id = matricula.Id,
                AlunoId = matricula.AlunoId,
                CursoId = matricula.CursoId,
                DataMatricula = matricula.DataMatricula,
                Ativa = matricula.Ativa,
                AlunoNome = matricula.Aluno?.Nome,
                CursoNome = matricula.Curso?.Nome
            };
        }
    }
}
