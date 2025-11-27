using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Handlers.Commands
{
    public class UpdateAlunoCommandHandler : IRequestHandler<UpdateAlunoCommand, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAlunoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AlunoDto> Handle(UpdateAlunoCommand request, CancellationToken cancellationToken)
        {
            var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
            if (aluno == null)
                throw new KeyNotFoundException($"Aluno com ID {request.Id} não encontrado");

            var emailExistente = await _unitOfWork.Alunos.VerificarEmailExistenteAsync(
                request.Email, request.Id, cancellationToken);

            if (emailExistente)
                throw new InvalidOperationException($"Email {request.Email} já está em uso por outro aluno");

            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);
            if (curso == null)
                throw new KeyNotFoundException($"Curso com ID {request.CursoId} não encontrado");

            aluno.AtualizarInformacoes(request.Nome, request.Email, request.DataNascimento);
            aluno.TrocarCurso(request.CursoId);

            _unitOfWork.Alunos.Update(aluno);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var alunoAtualizado = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);

            return MapToDto(alunoAtualizado);
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