using MediatR;

namespace Application.Commands
{
    public class AtivarCursoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public AtivarCursoCommand(int id)
        {
            Id = id;
        }
    }
}