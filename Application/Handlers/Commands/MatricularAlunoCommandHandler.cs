using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class MatricularAlunoCommandHandler : IRequestHandler<MatricularAlunoCommand, MatriculaDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MatricularAlunoCommandHandler> _logger;

        public MatricularAlunoCommandHandler(IUnitOfWork unitOfWork, ILogger<MatricularAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MatriculaDto> Handle(MatricularAlunoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Iniciando matrícula: AlunoId={request.AlunoId}, CursoId={request.CursoId}");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogDebug($"Validando aluno: AlunoId={request.AlunoId}");
                var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.AlunoId, cancellationToken);
                if (aluno == null)
                {
                    _logger.LogWarning($"Aluno não encontrado para matrícula: AlunoId={request.AlunoId}");
                    throw new InvalidOperationException($"Aluno com ID {request.AlunoId} não encontrado");
                }

                if (!aluno.Ativo)
                {
                    _logger.LogWarning($"Tentativa de matricular aluno inativo: AlunoId={aluno.Id}, Nome={aluno.Nome}");
                    throw new InvalidOperationException($"Aluno {aluno.Nome} está inativo");
                }

                _logger.LogDebug($"Validando curso: CursoId={request.CursoId}");
                var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);

                if (curso == null)
                {
                    _logger.LogWarning($"Curso não encontrado para matrícula: CursoId={request.CursoId}");
                    throw new InvalidOperationException($"Curso com ID {request.CursoId} não encontrado");
                }

                if (!curso.Ativo)
                {
                    _logger.LogWarning($"Tentativa de matrícula em curso inativo: CursoId={curso.Id}, Nome={curso.Nome}");
                    throw new InvalidOperationException($"Curso {curso.Nome} está inativo e não aceita matrículas");
                }

                _logger.LogDebug($"Verificando matrícula existente: AlunoId={request.AlunoId}, CursoId={request.CursoId}");
                var matriculaExistente = await _unitOfWork.Matriculas.ExisteMatriculaAtivaAsync(request.AlunoId, request.CursoId, cancellationToken);

                if (matriculaExistente)
                {
                    _logger.LogWarning($"Matrícula duplicada: AlunoId={request.AlunoId}, CursoId={request.CursoId}, Aluno={aluno.Nome}, Curso={curso.Nome}");
                    throw new InvalidOperationException($"Aluno já está matriculado no curso {curso.Nome}");
                }

                _logger.LogDebug($"Criando matrícula: Aluno={aluno.Nome}, Curso={curso.Nome}");
                var matricula = new Matricula(request.AlunoId, request.CursoId);

                await _unitOfWork.Matriculas.AddAsync(matricula);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();

                _logger.LogInformation($"Matrícula realizada com sucesso: MatriculaId={matricula.Id}, Aluno={aluno.Nome} (ID:{aluno.Id}), Curso={curso.Nome} (ID:{curso.Id}), Tempo={stopwatch.ElapsedMilliseconds}ms");

                var matriculaComDados = await _unitOfWork.Matriculas.GetByIdAsync(matricula.Id, cancellationToken);

                return MapToDto(matriculaComDados);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, $"Erro ao realizar matrícula: AlunoId={request.AlunoId}, CursoId={request.CursoId}, TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

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