using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetMatriculasPorAlunoQueryHandler : IRequestHandler<GetMatriculasPorAlunoQuery, List<MatriculaDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMatriculasPorAlunoQueryHandler> _logger;

        public GetMatriculasPorAlunoQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMatriculasPorAlunoQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<MatriculaDto>> Handle(GetMatriculasPorAlunoQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consultado matrícula por aluno");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var matriculas = await _unitOfWork.Matriculas.GetMatriculasPorAlunoAsync(request.Id, cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation($"Foram encontradas {matriculas.Count()} matrículas desse aluno");

                return matriculas.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogInformation(ex, $"Erro ao consultar matrícula por aluno");

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
