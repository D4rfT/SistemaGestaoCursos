using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class ReativarAlunoCommandHandler : IRequestHandler<ReativarAlunoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarAlunoCommandHandler> _logger;

        public ReativarAlunoCommandHandler(IUnitOfWork unitOfWork, ILogger<ReativarAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ReativarAlunoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reativando aluno ID={AlunoID}", request.Id);
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
                if (aluno == null)
                {
                    _logger.LogDebug("Aluno ID {AlunoID} não foi encontrado.", request.Id);
                    throw new InvalidOperationException($"Não existe Aluno com o id {request.Id}");
                }

                if (aluno.Ativo)
                {
                    _logger.LogDebug("Aluno ID {AlunoID} já está ativo.", request.Id);
                    throw new InvalidOperationException($"O Aluno do id {request.Id} já está ativo");
                }

                aluno.ReativarAluno();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation("Aluno ID {AlunoID} foi ativado.", request.Id);

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Erro ao ativar o aluno: AlunoId={AlunoId}, TempoDecorrido={TempoDecorrido}ms", request.Id, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
