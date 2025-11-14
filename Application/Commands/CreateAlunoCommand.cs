using MediatR;
using Application.Models;


namespace Application.Commands
{
    public class CreateAlunoCommand : IRequest<AlunoDto>
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RegistroAcademico { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public int CursoId { get; set; }

        public CreateAlunoCommand(string nome, string cPF, string registroAcademico, string email, DateTime dataNascimento, int cursoId)
        {
            Nome = nome;
            CPF = cPF;
            RegistroAcademico = registroAcademico;
            Email = email;
            DataNascimento = dataNascimento;
            CursoId = cursoId;
        }
    }
}
