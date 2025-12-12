using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetMatriculaByIdQueryHandler : IRequestHandler<GetMatriculaByIdQuery, MatriculaDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMatriculaByIdQueryHandler> _logger;

        public GetMatriculaByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMatriculaByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MatriculaDto> Handle(GetMatriculaByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consultado Matrícula ID {request.Id}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var matricula = await _unitOfWork.Matriculas.GetByIdAsync(request.Id, cancellationToken);
                stopwatch.Stop();

                if (matricula == null)
                {
                    _logger.LogDebug($"Matrícula com ID {request.Id} não encontrada");
                    throw new KeyNotFoundException($"Matrícula com ID {request.Id} não encontrada");
                }

                _logger.LogInformation($"Matrícula ID {request.Id} encontrada.");

                return MapToDto(matricula);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogInformation(ex, $"Erro ao consultar o ID {request.Id}");

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
