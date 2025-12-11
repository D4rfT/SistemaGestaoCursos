using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class DesativarCursoCommandHandler : IRequestHandler<DesativarCursoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarAlunoCommandHandler> _logger;

        public DesativarCursoCommandHandler(IUnitOfWork unitOfWork, ILogger<ReativarAlunoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DesativarCursoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Desativando curso ID={request.Id}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var curso = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);
                if (curso == null)
                {
                    _logger.LogDebug($"Curso ID {request.Id} não foi encontrando");
                    throw new InvalidOperationException($"Não existe nenhum curso com o id'{request.Id}'");
                }
                if (!curso.Ativo)
                {
                    _logger.LogDebug($"Curso ID {request.Id} já está desativado.");
                    throw new InvalidOperationException($"O Curso {curso.Nome} já está desativado.");
                }
                curso.Desativar();

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
