using MediatR;

namespace Application.Commands
{
    public class DeleteCursoCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteCursoCommand(int id)
        {
            Id = id;
        }
    }
}