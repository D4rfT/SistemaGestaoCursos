using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Queries
{
    public class GetAllCursosQueryHandler : IRequestHandler<GetAllCursosQuery, List<CursoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCursosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CursoDto>> Handle(GetAllCursosQuery request, CancellationToken cancellationToken)
        {
            var cursos = await _unitOfWork.Cursos.GetAllAsync(cancellationToken);

            return cursos.Select(MapToDto).ToList();
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
