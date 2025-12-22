using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace Application.Handlers.Queries
{
    public class GetCursoByIdQueryHandler : IRequestHandler<GetCursoByIdQuery, CursoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCursoByIdQueryHandler> _logger;

        public GetCursoByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCursoByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CursoDto> Handle(GetCursoByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consultado Curso ID {request.Id}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var curso = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);
                stopwatch.Stop();

                if (curso == null)
                {
                    _logger.LogDebug($"Curso ID {request.Id} não encontrado");
                    throw new KeyNotFoundException($"Curso com ID {request.Id} não encontrado");
                }

                _logger.LogInformation($"Curso ID {request.Id} encontrado. Nome: {curso.Nome}, Preço:{curso.Preco}, Duração: {curso.Duracao}, Ativo: {curso.Ativo}");
                return MapToDto(curso);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro ao consultar o ID {request.Id}");

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
