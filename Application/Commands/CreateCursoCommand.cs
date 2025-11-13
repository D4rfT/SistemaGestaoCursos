using Application.Models;
using MediatR;

namespace Application.Commands
{
    public class CreateCursoCommand : IRequest<CursoDto>
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Duracao { get; set; }

        public CreateCursoCommand(string nome, string descricao, decimal preco, int duracao)
        {
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Duracao = duracao;
        }
    }
}