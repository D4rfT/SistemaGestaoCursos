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
            _logger.LogInformation("Iniciando criação de curso: Nome={Nome}, Descricao={Descricao}, Preco={Preco}, Duracao={Duracao}h", request.Nome, request.Descricao, request.Preco, request.Duracao);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogDebug("Verificando se o curso '{Nome}' já existe", request.Nome);
                var cursoExistente = await _unitOfWork.Cursos.GetByNomeAsync(request.Nome, cancellationToken);

                if (cursoExistente != null)
                {
                    _logger.LogWarning("Tentativa de criar curso com nome duplicado: Nome={Nome}, CursoExistenteId={CursoExistenteId}", request.Nome, cursoExistente.Id);
                    throw new InvalidOperationException($"Já existe um curso com o nome '{request.Nome}'");
                }

                _logger.LogDebug("Criando entidade Curso: Nome={Nome}", request.Nome);
                var novoCurso = new Curso(request.Nome, request.Descricao, request.Preco, request.Duracao);

                _logger.LogDebug("Adicionando curso ao repositório");
                await _unitOfWork.Cursos.AddAsync(novoCurso, cancellationToken);

                _logger.LogDebug("Salvando alterações no banco");
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                stopwatch.Stop();

                _logger.LogInformation("Curso criado com sucesso: ID={CursoId}, Nome={Nome}, Tempo={Tempo}ms", novoCurso.Id, request.Nome, stopwatch.ElapsedMilliseconds);
                var cursoCriado = await _unitOfWork.Cursos.GetByIdAsync(novoCurso.Id, cancellationToken);

                return MapToDto(cursoCriado);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Erro ao criar curso: Nome={Nome}, Preco={Preco}, TempoDecorrido={TempoDecorrido}ms", request.Nome, request.Preco, stopwatch.ElapsedMilliseconds);
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