using MediatR;


namespace Application.Commands
{
    public class ReativarMatriculaCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public ReativarMatriculaCommand(int id)
        {
            Id = id;
        }
    }
}
