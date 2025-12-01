using MediatR;
using Application.Models;

namespace Application.Commands
{
    public class MatricularAlunoCommand : IRequest<MatriculaDto>
    {
        public int AlunoId { get; set; }
        public int CursoId { get; set; }



        public MatricularAlunoCommand(int alunoId, int cursoId)
        {
            AlunoId = alunoId;
            CursoId = cursoId;
        }
    }
}
