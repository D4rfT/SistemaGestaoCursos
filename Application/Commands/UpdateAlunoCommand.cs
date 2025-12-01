using Application.Models;
using MediatR;

namespace Application.Commands
{
    public class UpdateAlunoCommand : IRequest<AlunoDto>
    {
        public int Id { get; init; }
        public string Nome { get; init; }
        public string Email { get; init; }
        public DateTime DataNascimento { get; init; }
        public int CursoId { get; init; }


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