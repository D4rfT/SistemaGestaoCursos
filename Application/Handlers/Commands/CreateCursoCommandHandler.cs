using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Commands
{
    public class CreateCursoCommandHandler : IRequestHandler<CreateCursoCommand, CursoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCursoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CursoDto> Handle(CreateCursoCommand request, CancellationToken cancellationToken)
        {
            var cursoExistente = await _unitOfWork.Cursos.GetByNomeAsync(request.Nome, cancellationToken);
            if (cursoExistente != null)
                throw new InvalidOperationException($"Já existe um curso com o nome '{request.Nome}'");

            var novoCurso = new Curso(request.Nome, request.Descricao, request.Preco, request.Duracao);

            await _unitOfWork.Cursos.AddAsync(novoCurso, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var cursoCriado = await _unitOfWork.Cursos.GetByIdAsync(novoCurso.Id, cancellationToken);

            return MapToDto(cursoCriado);
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