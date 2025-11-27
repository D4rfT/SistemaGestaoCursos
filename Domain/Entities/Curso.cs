using Domain.Common;

namespace Domain.Entities
{
    public class Curso : BaseEntity
    {
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public decimal Preco { get; private set; }
        public int Duracao { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCriacao { get; private set; }
        public ICollection<Aluno> Alunos { get; private set; } = new List<Aluno>();

        private Curso() { }

        public Curso(string nome, string descricao, decimal preco, int duracao)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            Preco = preco >= 0 ? preco : throw new ArgumentException("Preço não pode ser negativo");
            Duracao = duracao > 0 ? duracao : throw new ArgumentException("Duração deve ser positiva");
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
        }

        public void AtualizarInformacoes(string nome, string descricao, decimal preco, int duracao)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            Preco = preco >= 0 ? preco : throw new ArgumentException("Preço não pode ser negativo");
            Duracao = duracao > 0 ? duracao : throw new ArgumentException("Duração deve ser positiva");
        }

        public void Desativar() => Ativo = false;

        public void Ativar() => Ativo = true;

    }
}