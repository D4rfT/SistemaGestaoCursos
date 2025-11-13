using MediatR;

namespace Application.Commands
{
    public class DesativarCursoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DesativarCursoCommand(int id)
        {
            Id = id;
        }
    }
}