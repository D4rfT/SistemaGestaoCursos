using Application.Models;

namespace Application.Models
{
    public class MatriculaDto : BaseDto
    {
        public int AlunoId { get; set; }
        public int CursoId { get; set; }
        public DateTime DataMatricula { get; set; }
        public bool Ativa { get; set; }

        public MatriculaDto() { }

    }
}
