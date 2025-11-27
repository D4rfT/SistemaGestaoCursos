using MediatR;

namespace Application.Commands
{
    public class DesativarMatriculaCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DesativarMatriculaCommand(int id)
        {
            Id = id;
        }
    }
}
