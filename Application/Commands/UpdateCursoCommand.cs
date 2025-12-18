using Application.Models;
using MediatR;

namespace Application.Commands
{
    public class UpdateCursoCommand : IRequest<CursoDto>
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Duracao { get; set; }

        public UpdateCursoCommand() { }

        public UpdateCursoCommand(int id, string nome, string descricao, decimal preco, int duracao)
        {
            Id = id;
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Duracao = duracao;
        }
    }
}