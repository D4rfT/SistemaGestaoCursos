using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace Application.Handlers.Commands
{
    public class DesativarMatriculaCommandHandler : IRequestHandler<DesativarMatriculaCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarMatriculaCommandHandler> _logger;

        public DesativarMatriculaCommandHandler(IUnitOfWork unitOfWork, ILogger<ReativarMatriculaCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DesativarMatriculaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Reativando matrícula ID={request.Id}");
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var matricula = await _unitOfWork.Matriculas.GetByIdAsync(request.Id, cancellationToken);
                if (matricula == null)
                {
                    _logger.LogDebug($"Matrícula ID {request.Id} não foi encontranda");
                    throw new InvalidOperationException($"Não existe nenhuma matrícula com o id'{request.Id}'");
                }

                if (!matricula.Ativa)
                {
                    _logger.LogDebug("Matrícula ID {MatriculaId} já está desativada.", request.Id);
                    throw new InvalidOperationException($"A matrícula de id {matricula.Id} já está desativada.");
                }

                matricula.CancelarMatricula();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation("Matrícula ID {request.Id} foi desativada.");

                return true;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, $"Erro ao desativar a matrícula: MatrículaId={request.Id}, TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

                throw;
            }
        }
    }
}
