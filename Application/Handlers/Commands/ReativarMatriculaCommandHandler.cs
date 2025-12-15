using Application.Commands;
using Application.Interfaces;
using AutoMapper.Internal;
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

                if (matricula.Ativa)
                {
                    _logger.LogDebug($"Matrícula ID {request.Id} já está ativa.");
                    throw new InvalidOperationException($"A matrícula do id {request.Id} já está ativa.");
                }

                matricula.ReativarMatricula();
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation($"Matrícula ID {request.Id} foi ativada.");

                return true;
            }

            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, $"Erro ao ativar a matrícula: MatrículaId={request.Id}, TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");

                throw;
            }
        }

    }
}
