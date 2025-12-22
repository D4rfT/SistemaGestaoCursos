using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetMatriculasPorCursoQueryHandler : IRequestHandler<GetMatriculasPorCursoQuery, List<MatriculaDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMatriculasPorCursoQueryHandler> _logger;

        public GetMatriculasPorCursoQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMatriculasPorCursoQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<MatriculaDto>> Handle(GetMatriculasPorCursoQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consultado matrícula por curso");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var matriculas = await _unitOfWork.Matriculas.GetMatriculasPorCursoAsync(request.Id, cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation($"Foram encontradas {matriculas.Count()} matrículas nesse curso");

                return matriculas.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar matrícula por curso");

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
