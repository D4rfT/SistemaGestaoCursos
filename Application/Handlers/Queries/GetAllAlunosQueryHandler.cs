using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetAllAlunosQueryHandler : IRequestHandler<GetAllAlunosQuery, List<AlunoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllAlunosQueryHandler> _logger;

        public GetAllAlunosQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllAlunosQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<AlunoDto>> Handle(GetAllAlunosQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Iniciando consulta de todos os alunos");
            var stopwatch = Stopwatch.StartNew();

            try
            {            
                var alunos = await _unitOfWork.Alunos.GetAllAsync(cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation($"Consulta de alunos concluída: TotalAlunos={alunos.Count()}, TempoExecucao={stopwatch.ElapsedMilliseconds}ms");

                if(alunos.Count() == 0)
                {
                    _logger.LogWarning("Consulta de alunos retornou 0 resultados");
                }
                else
                {
                    _logger.LogDebug("Alunos encontrados: {@Alunos}", alunos.Select(a => new { a.Id, a.Nome, a.CPF }).Take(5));

                    var alunosAtivos = alunos.Count(a => a.Ativo);
                    var alunosInativos = alunos.Count(a => !a.Ativo);

                    _logger.LogDebug($"Estatísticas: Ativos={alunosAtivos}, Inativos={alunosInativos}");
                }
                    return alunos.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar Alunos: TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

                throw;
            }
        }

        private AlunoDto MapToDto(Aluno aluno)
        {
            return new AlunoDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                CPF = aluno.CPF,
                RegistroAcademico = aluno.RegistroAcademico,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                Ativo = aluno.Ativo,
                CursoId = aluno.CursoId
            };
        }

    }
}
