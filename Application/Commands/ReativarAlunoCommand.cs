using MediatR;

namespace Application.Commands
{
    public class ReativarAlunoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public ReativarAlunoCommand(int id)
        {
            Id = id;
        }
    }
}
