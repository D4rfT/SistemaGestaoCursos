using Application.Models;

namespace Application.Models
{
    public class AlunoDto : BaseDto
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RegistroAcademico { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool Ativo { get; set; }
        public int CursoId { get; set; }

        public AlunoDto() { }

    }
}
