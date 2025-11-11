namespace Domain.Entities
{
    public class Matricula
    {
        public string NumeroMatricula { get; private set; }
        public int AlunoId { get; private set; }
        public int CursoId { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public bool Ativa { get; private set; }

        private Matricula(){}

        public Matricula(string numeroMatricula, int alunoId, int cursoId) 
        {
            NumeroMatricula = numeroMatricula ?? throw new ArgumentNullException(nameof(numeroMatricula));
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
