using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Queries
{
    public class GetAlunoByIdQueryHandler : IRequestHandler<GetAlunoByIdQuery, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAlunoByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AlunoDto> Handle(GetAlunoByIdQuery request, CancellationToken cancellationToken)
        {
            var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);

            if (aluno == null)
                throw new KeyNotFoundException($"Aluno com ID {request.Id} não encontrado");

            return MapToDto(aluno);
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
