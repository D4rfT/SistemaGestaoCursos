using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class ReativarCursoCommandHandler : IRequestHandler<ReativarCursoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarAlunoCommandHandler> _logger;

        public ReativarCursoCommandHandler(IUnitOfWork unitOfWork, ILogger<ReativarAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ReativarCursoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reativando curso ID={CursoID}", request.Id);
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var curso = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);
                if (curso == null)
                {
                    _logger.LogDebug("Curso ID {CursoId} não foi encontrando", request.Id);
                    throw new InvalidOperationException($"Não existe nenhum curso com o id'{request.Id}'");
                }

                if (curso.Ativo)
                {
                    _logger.LogDebug("Curso ID {CursoId} já está ativo.");
                    throw new InvalidOperationException($"O Curso {curso.Nome} já está ativo.");
                }

                curso.Ativar();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation("Curso ID {AlunoID} foi ativado.", request.Id);

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Erro ao ativar o aluno: CursoId={AlunoId}, TempoDecorrido={TempoDecorrido}ms", request.Id, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
