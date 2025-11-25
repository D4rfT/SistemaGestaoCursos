using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Queries
{
    public class GetCursosComFiltrosQueryHandler : IRequestHandler<GetCursosComFiltrosQuery, List<CursoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCursosComFiltrosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CursoDto>> Handle(GetCursosComFiltrosQuery request, CancellationToken cancellationToken)
        {
            var cursos = await _unitOfWork.Cursos.GetCursosComFiltrosAsync(
                nome: request.Nome,
                precoMinimo: request.PrecoMinimo,
                precoMaximo: request.PrecoMaximo,
                duracaoMinima: request.DuracaoMinima,
                duracaoMaxima: request.DuracaoMaxima,
                ativo: request.Ativo,
                ordenarPor: request.OrdenarPor,
                ordemDescendente: request.OrdemDescendente,
                cancellationToken: cancellationToken
            );

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