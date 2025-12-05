using Application.Interfaces;
using Application.Models;
using Application.Queries;
using MediatR;

namespace Application.Handlers.Queries
{
    public class GetAlunoByUsuarioIdQueryHandler : IRequestHandler<GetAlunoByUsuarioIdQuery, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAlunoByUsuarioIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AlunoDto> Handle(GetAlunoByUsuarioIdQuery request, CancellationToken cancellationToken)
        {
            var aluno = await _unitOfWork.Alunos
                .FindAsync(a => a.UsuarioId == request.UsuarioId, cancellationToken)
                .ContinueWith(task => task.Result.FirstOrDefault());

            if (aluno == null)
                throw new KeyNotFoundException($"Aluno não encontrado para o usuário ID {request.UsuarioId}");

            return MapToDto(aluno);
        }

        private AlunoDto MapToDto(Domain.Entities.Aluno aluno)
        {
            return new AlunoDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                CPF = aluno.CPF,
                RegistroAcademico = aluno.RegistroAcademico,
                Email = aluno.Email,
                DataNascimento = aluno.DataNascimento,
                Ativo = aluno.Ativo,
                CursoId = aluno.CursoId
            };
        }
    }
}