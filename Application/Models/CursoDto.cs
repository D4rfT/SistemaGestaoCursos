using Application.Models;

namespace Application.Models
{
    public class CursoDto : BaseDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int Duracao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }


        public CursoDto() { } // Construtor vazio para serialização

    }
}