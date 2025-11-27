using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Commands
{
    public class CreateAlunoCommandHandler : IRequestHandler<CreateAlunoCommand, AlunoDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAlunoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AlunoDto> Handle(CreateAlunoCommand request, CancellationToken cancellationToken)
        {
            var cpfExistente = await _unitOfWork.Alunos.VerificarCpfExistenteAsync(request.CPF, cancellationToken: cancellationToken);
            if (cpfExistente)
                throw new InvalidOperationException($"Já existe um aluno com o CPF {request.CPF}");

            var emailExistente = await _unitOfWork.Alunos.VerificarEmailExistenteAsync(request.Email, cancellationToken: cancellationToken);
            if (emailExistente)
                throw new InvalidOperationException($"Já existe um aluno com o email {request.Email}");

            var raExistente = await _unitOfWork.Alunos.GetByRegistroAcademicoAsync(request.RegistroAcademico, cancellationToken);
            if (raExistente != null)
                throw new InvalidOperationException($"Já existe um aluno com o registro acadêmico {request.RegistroAcademico}");

            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);
            if (curso == null)
                throw new InvalidOperationException($"Curso com ID {request.CursoId} não encontrado");     

            var novoAluno = new Aluno(request.Nome, request.CPF, request.RegistroAcademico, request.Email, request.DataNascimento, request.CursoId);

            await _unitOfWork.Alunos.AddAsync(novoAluno, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var alunoCriado = await _unitOfWork.Alunos.GetByIdAsync(novoAluno.Id, cancellationToken);

            return MapToDto(alunoCriado);
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
                Ativo = aluno.Ativo,
                CursoId = aluno.CursoId,
            };
        }
    }
}
