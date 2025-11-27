using Application.Models;
using MediatR;

namespace Application.Commands
{
    public class UpdateAlunoCommand : IRequest<AlunoDto>
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public int CursoId { get; set; }

        public UpdateAlunoCommand(int id, string nome, string email, DateTime dataNascimento, int cursoId)
        {
            Id = id;
            Nome = nome;
            Email = email;
            DataNascimento = dataNascimento;
            CursoId = cursoId;
        }
    }
}