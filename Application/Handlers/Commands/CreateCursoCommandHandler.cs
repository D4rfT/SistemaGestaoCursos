using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Handlers.Commands
{
    public class CreateCursoCommandHandler : IRequestHandler<CreateCursoCommand, CursoDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCursoCommandHandler> _logger;

        public CreateCursoCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateCursoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CursoDto> Handle(CreateCursoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Iniciando criação de curso: Nome={request.Nome}, Descricao={request.Descricao}, Preco={request.Preco}, Duracao={request.Duracao}h");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogDebug($"Verificando se o curso '{request.Nome}' já existe");
                var cursoExistente = await _unitOfWork.Cursos.GetByNomeAsync(request.Nome, cancellationToken);

                if (cursoExistente != null)
                {
                    _logger.LogWarning($"Tentativa de criar curso com nome duplicado: Nome={request.Nome}, CursoExistenteId={cursoExistente.Id}");
                    throw new InvalidOperationException($"Já existe um curso com o nome '{request.Nome}'");
                }

                _logger.LogDebug($"Criando entidade Curso: Nome={request.Nome}");
                var novoCurso = new Curso(request.Nome, request.Descricao, request.Preco, request.Duracao);


                await _unitOfWork.Cursos.AddAsync(novoCurso, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();

                _logger.LogInformation($"Curso criado com sucesso: ID={novoCurso.Id}, Nome={request.Nome}, Tempo={stopwatch.ElapsedMilliseconds}ms");
                var cursoCriado = await _unitOfWork.Cursos.GetByIdAsync(novoCurso.Id, cancellationToken);

                return MapToDto(cursoCriado);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, $"Erro ao criar curso: Nome={request.Nome}, Preco={request.Preco}, TempoDecorrido={stopwatch.ElapsedMilliseconds}ms");
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