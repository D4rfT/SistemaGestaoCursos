using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetAlunoByIdQueryHandler : IRequestHandler<GetAlunoByIdQuery, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAlunoByIdQueryHandler> _logger;

        public GetAlunoByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAlunoByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AlunoDto> Handle(GetAlunoByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consultado aluno ID {request.Id}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
                stopwatch.Stop();

                if (aluno == null)
                {
                    _logger.LogDebug($"Aluno ID {request.Id} não encontrado");
                    throw new KeyNotFoundException($"Aluno com ID {request.Id} não encontrado");
                }

                _logger.LogInformation($"Aluno ID {request.Id} encontrado. Nome: {aluno.Nome} CPF: {aluno.CPF} Email: {aluno.Email}");

                return MapToDto(aluno);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar o ID {request.Id}");

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
