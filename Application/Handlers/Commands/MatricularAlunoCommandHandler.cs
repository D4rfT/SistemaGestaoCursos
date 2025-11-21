using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Commands
{
    public class MatricularAlunoCommandHandler : IRequestHandler<MatricularAlunoCommand, MatriculaDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public MatricularAlunoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MatriculaDto> Handle(MatricularAlunoCommand request, CancellationToken cancellationToken)
        {
            var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.AlunoId, cancellationToken);
            if (aluno == null)
                throw new InvalidOperationException($"Aluno com ID {request.AlunoId} não encontrado");

            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.CursoId, cancellationToken);
            if (curso == null)
                throw new InvalidOperationException($"Curso com ID {request.CursoId} não encontrado");

            if (!curso.Ativo)
                throw new InvalidOperationException($"Curso {curso.Nome} está inativo e não aceita matrículas");

            var matriculaExistente = await _unitOfWork.Matriculas
                .ExisteMatriculaAtivaAsync(request.AlunoId, request.CursoId, cancellationToken);

            if (matriculaExistente)
                throw new InvalidOperationException($"Aluno já está matriculado no curso {curso.Nome}");

            var matricula = new Matricula(request.AlunoId, request.CursoId);

            await _unitOfWork.Matriculas.AddAsync(matricula);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var matriculaComDados = await _unitOfWork.Matriculas
                .GetByIdAsync(matricula.Id, cancellationToken);

            return MapToDto(matriculaComDados);
        }

        private MatriculaDto MapToDto(Matricula matricula)
        {
            return new MatriculaDto
            {
                Id = matricula.Id,
                AlunoId = matricula.AlunoId,
                CursoId = matricula.CursoId,
                DataMatricula = matricula.DataMatricula,
                Ativa = matricula.Ativa,
                AlunoNome = matricula.Aluno?.Nome,
                CursoNome = matricula.Curso?.Nome
            };
        }
    }
}