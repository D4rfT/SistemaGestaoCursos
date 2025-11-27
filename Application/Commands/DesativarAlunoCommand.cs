using MediatR;

namespace Application.Commands
{
    public class DesativarAlunoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DesativarAlunoCommand(int id)
        {
            Id = id;
        }
    }
}
