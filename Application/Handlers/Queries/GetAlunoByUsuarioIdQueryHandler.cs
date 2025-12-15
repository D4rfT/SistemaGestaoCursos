using Application.Interfaces;
using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Queries
{
    public class GetAlunoByUsuarioIdQueryHandler : IRequestHandler<GetAlunoByUsuarioIdQuery, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAlunoByUsuarioIdQueryHandler> _logger;

        public GetAlunoByUsuarioIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAlunoByUsuarioIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AlunoDto> Handle(GetAlunoByUsuarioIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consultado aluno por usuário");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var aluno = await _unitOfWork.Alunos.FindAsync(a => a.UsuarioId == request.UsuarioId, cancellationToken).ContinueWith(task => task.Result.FirstOrDefault());
                stopwatch.Stop();

                if (aluno == null)
                {
                    _logger.LogDebug($"Aluno não encontrado para o usuário ID {request.UsuarioId}");
                    throw new KeyNotFoundException($"Aluno não encontrado para o usuário ID {request.UsuarioId}");
                }

                _logger.LogInformation($"Aluno ID {aluno.Id} encontrado para o usuário ID {request.UsuarioId}. Nome: {aluno.Nome} CPF: {aluno.CPF}, Email: {aluno.Email}, Ativo: {aluno.Ativo}");

                return MapToDto(aluno);

            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogInformation(ex, $"Erro ao consultar o ID {request.UsuarioId}");

                throw;
            }
        }

        private AlunoDto MapToDto(Domain.Entities.Aluno aluno)
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