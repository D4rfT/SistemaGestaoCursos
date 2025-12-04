using Domain.Common;

namespace Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string SenhaHash { get; private set; }
        public string Role { get; private set; }
        public bool Ativo { get; private set; }

        private Usuario() { }

        public Usuario(string nome, string email, string senhaHash, string role)
        {
            Nome = nome;
            Email = email;
            SenhaHash = senhaHash;
            Role = role;
            Ativo = true;
        }

        public bool ValidarSenha(string senhaHash) => SenhaHash == senhaHash;
    }
}