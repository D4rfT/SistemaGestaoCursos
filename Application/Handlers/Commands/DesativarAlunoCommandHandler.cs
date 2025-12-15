using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class DesativarAlunoCommandHandler : IRequestHandler<DesativarAlunoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarAlunoCommandHandler> _logger;

        public DesativarAlunoCommandHandler(IUnitOfWork unitOfWork, ILogger<DesativarAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DesativarAlunoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Desativando aluno ID={request.Id}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
                if (aluno == null)
                {
                    _logger.LogDebug($"Aluno ID {request.Id} não foi encontrado.");
                    throw new InvalidOperationException($"Não existe Aluno com o id {request.Id}");
                }

                if (!aluno.Ativo)
                {
                    _logger.LogDebug($"Aluno ID {request.Id} já está desativado.");
                    throw new InvalidOperationException($"O Aluno de id {request.Id} já está desativado");
                }

                aluno.DesativarAluno();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation($"Aluno ID {request.Id} foi desativado.");

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, $"Erro ao desativar o aluno: AlunoId={request.Id}, TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

                throw;
            }
        }
    }
}
