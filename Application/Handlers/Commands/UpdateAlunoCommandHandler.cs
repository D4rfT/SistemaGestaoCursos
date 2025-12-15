using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class UpdateAlunoCommandHandler : IRequestHandler<UpdateAlunoCommand, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateAlunoCommandHandler> _logger;

        public UpdateAlunoCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<AlunoDto> Handle(UpdateAlunoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Atualizando aluno ID={request.Id}");
            var stopwatch = Stopwatch.StartNew();
            try
            {

                var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
                if (aluno == null)
                {
                    _logger.LogWarning($"Aluno não encontrado para atualização: ID={request.Id}");
                    throw new KeyNotFoundException($"Aluno com ID {request.Id} não encontrado");
                }

                var emailExistente = await _unitOfWork.Alunos.VerificarEmailExistenteAsync(request.Email, request.Id, cancellationToken);
                if (emailExistente)
                {
                    _logger.LogWarning($"Email já em uso: Email={request.Email}");
                    throw new InvalidOperationException($"Email {request.Email} já está em uso por outro aluno");
                }

                var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);
                if (curso == null)
                {
                    _logger.LogWarning($"Curso não encontrado: CursoId={request.CursoId}");
                    throw new KeyNotFoundException($"Curso com ID {request.CursoId} não encontrado");
                }

                aluno.AtualizarInformacoes(request.Nome, request.Email, request.DataNascimento.ToUniversalTime());
                aluno.TrocarCurso(request.CursoId);

                _unitOfWork.Alunos.Update(aluno);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation($"Aluno atualizado: ID={request.Id}, Tempo={stopwatch.ElapsedMilliseconds}ms");           
                var alunoAtualizado = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);

                return MapToDto(alunoAtualizado);
            }

            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao atualizar aluno: ID={request.Id}, Tempo={stopwatch.ElapsedMilliseconds}ms");
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