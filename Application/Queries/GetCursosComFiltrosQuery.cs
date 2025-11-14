using Application.Models;
using MediatR;

namespace Application.Queries
{
    public class GetCursosComFiltrosQuery : IRequest<List<CursoDto>>
    {
        public string? Nome { get; set; }
        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }
        public int? DuracaoMinima { get; set; }
        public int? DuracaoMaxima { get; set; }
        public bool? Ativo { get; set; }
        public string? OrdenarPor { get; set; }
        public bool OrdemDescendente { get; set; }

        public GetCursosComFiltrosQuery(
            string? nome = null,
            decimal? precoMinimo = null,
            decimal? precoMaximo = null,
            int? duracaoMinima = null,
            int? duracaoMaxima = null,
            bool? ativo = null,
            string? ordenarPor = null,
            bool ordemDescendente = false)
        {
            Nome = nome;
            PrecoMinimo = precoMinimo;
            PrecoMaximo = precoMaximo;
            DuracaoMinima = duracaoMinima;
            DuracaoMaxima = duracaoMaxima;
            Ativo = ativo;
            OrdenarPor = ordenarPor;
            OrdemDescendente = ordemDescendente;
        }
    }
}