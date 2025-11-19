using Domain.Common;

namespace Domain.Entities
{
    public class Matricula :BaseEntity
    {
        public int AlunoId { get; private set; }
        public int CursoId { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public bool Ativa { get; private set; }
        public Aluno Aluno { get; private set; }
        public Curso Curso { get; private set; }

        private Matricula(){}

        public Matricula(int alunoId, int cursoId) 
        {
            AlunoId = alunoId > 0 ? alunoId : throw new ArgumentException("ID do aluno inválido");
            CursoId = cursoId > 0 ? cursoId : throw new ArgumentException("ID do curso inválido");
            DataMatricula = DateTime.UtcNow;
            Ativa = true;
        }

        public void CancelarMatricula() => Ativa = false;
        public void ReativarMatricula() => Ativa = true;
        public bool MatriculaEstaAtiva() => Ativa;
    }
}
