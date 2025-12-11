using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class UpdateCursoCommandHandler : IRequestHandler<UpdateCursoCommand, CursoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCursoCommandHandler> _logger;

        public UpdateCursoCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateCursoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CursoDto> Handle(UpdateCursoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Atualizando Curso ID={CursoId}", request.Id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var curso = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);
                if (curso == null)
                {
                    _logger.LogWarning("Curso não encontrado para atualização: ID={CursoId}", request.Id);
                    throw new InvalidOperationException($"Curso com ID {request.Id} não encontrado");
                }

                curso.AtualizarInformacoes(request.Nome, request.Descricao, request.Preco, request.Duracao);

                _unitOfWork.Cursos.Update(curso);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();
                _logger.LogInformation("Curso atualizado: ID={CursoId}, Tempo={Tempo}ms", request.Id, stopwatch.ElapsedMilliseconds);
                var cursoAtualizado = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);

                return MapToDto(cursoAtualizado);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Erro ao atualizar curso: ID={CursoId}, Tempo={Tempo}ms", request.Id, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        private CursoDto MapToDto(Curso curso)
        {
            return new CursoDto
            {
                Id = curso.Id,
                Nome = curso.Nome,
                Descricao = curso.Descricao,
                Preco = curso.Preco,
                Duracao = curso.Duracao,
                Ativo = curso.Ativo,
                DataCriacao = curso.DataCriacao
            };
        }
    }
}
