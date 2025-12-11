using Application.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class ReativarMatriculaCommandHandler : IRequestHandler<ReativarMatriculaCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarMatriculaCommandHandler> _logger;

        public ReativarMatriculaCommandHandler(IUnitOfWork unitOfWork, ILogger<ReativarMatriculaCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ReativarMatriculaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reativando matrícula ID={AlunoID}", request.Id);
            var stopwatch = Stopwatch.StartNew();
            try
            {

                var matricula = await _unitOfWork.Matriculas.GetByIdAsync(request.Id, cancellationToken);
                if (matricula == null)
                {
                    _logger.LogDebug("Matrícula ID {MatriculaId} não foi encontranda", request.Id);
                    throw new InvalidOperationException($"Não existe nenhuma matrícula com o id'{request.Id}'");
                }

                if (matricula.Ativa)
                {
                    _logger.LogDebug("Matrícula ID {MatriculaId} já está ativa.", request.Id);
                    throw new InvalidOperationException($"A matrícula do id {request.Id} já está ativa.");
                }

                matricula.ReativarMatricula();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation("Matrícula ID {MatriculaID} foi ativada.", request.Id);

                return true;
            }

            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Erro ao ativar a matrícula: MatrículaId={MatriculaId}, TempoDecorrido={TempoDecorrido}ms", request.Id, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

    }
}
