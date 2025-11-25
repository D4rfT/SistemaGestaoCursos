using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;



namespace Application.Handlers.Queries
{
    public class GetAllAlunosQueryHandler : IRequestHandler<GetAllAlunosQuery, List<AlunoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllAlunosQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AlunoDto>> Handle(GetAllAlunosQuery request, CancellationToken cancellationToken)
        {
            var alunos = await _unitOfWork.Alunos.GetAllAsync(cancellationToken);

            return alunos.Select(MapToDto).ToList();
        }

        private AlunoDto MapToDto(Aluno aluno)
        {
            return new AlunoDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                CPF = aluno.CPF,
                RegistroAcademico = aluno.RegistroAcademico,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                CursoId = aluno.CursoId
            };
        }

    }
}
