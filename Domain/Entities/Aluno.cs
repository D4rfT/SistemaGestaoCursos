using Domain.Common;

namespace Domain.Entities
{
    public class Aluno : BaseEntity
    {
        public string Nome { get; private set; }
        public string CPF { get; private set; }
        public string RegistroAcademico { get; private set; }
        public string Email { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public int CursoId { get; private set; }
        public bool Ativo { get; private set; }
        public Curso Curso { get; private set; }


        private Aluno() { }

        public Aluno(string nome, string cpf, string registroAcademico, string email, DateTime dataNascimento, int cursoId)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            CPF = ValidarCPF(cpf);
            RegistroAcademico = registroAcademico ?? throw new ArgumentNullException(nameof(registroAcademico));
            Email = ValidarEmail(email);
            DataNascimento = ValidarDataNascimento(dataNascimento);
            CursoId = cursoId > 0 ? cursoId : throw new ArgumentException("ID do curso inválido");
            Ativo = true;
        }

        public Aluno(string nome, string cpf, string email, DateTime dataNascimento, int cursoId)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            CPF = ValidarCPF(cpf);
            Email = ValidarEmail(email);
            DataNascimento = ValidarDataNascimento(dataNascimento);
            CursoId = cursoId > 0 ? cursoId : throw new ArgumentException("ID do curso inválido");
            Ativo = true;

            RegistroAcademico = "TEMP";
        }

        public void AtualizarInformacoes(string nome, string email, DateTime dataNascimento)
        {
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Email = ValidarEmail(email);
            DataNascimento = ValidarDataNascimento(dataNascimento);
        }

        public void TrocarCurso(int novoCursoId)
        {
            CursoId = novoCursoId > 0 ? novoCursoId : throw new ArgumentException("ID do curso inválido");
        }

        private string ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                throw new ArgumentException("CPF deve conter 11 dígitos");

            return cpf;
        }

        private string ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email inválido");

            return email;
        }

        private DateTime ValidarDataNascimento(DateTime dataNascimento)
        {
            var idade = DateTime.UtcNow.Year - dataNascimento.Year;

            if (dataNascimento > DateTime.UtcNow)
                throw new ArgumentException("Data de nascimento não pode ser futura");

            if (idade < 16)
                throw new ArgumentException("Aluno deve ter pelo menos 16 anos");

            return dataNascimento;
        }

        public void DesativarAluno() => Ativo = false;

        public void ReativarAluno() => Ativo = true;

    }
}
