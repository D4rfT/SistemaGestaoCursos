using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Commands
{
    public class UpdateCursoCommandHandler : IRequestHandler<UpdateCursoCommand, CursoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCursoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CursoDto> Handle(UpdateCursoCommand request, CancellationToken cancellationToken)
        {
            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);
            if (curso == null)
                throw new InvalidOperationException($"Curso com ID {request.Id} não encontrado");

            curso.AtualizarInformacoes(request.Nome, request.Descricao, request.Preco, request.Duracao);

            _unitOfWork.Cursos.Update(curso);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var cursoAtualizado = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);

            return MapToDto(cursoAtualizado);
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
