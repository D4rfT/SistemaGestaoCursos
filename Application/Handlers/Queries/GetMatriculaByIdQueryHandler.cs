using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;
using System.Threading;

namespace Application.Handlers.Queries
{
    public class GetMatriculaByIdQueryHandler : IRequestHandler<GetMatriculaByIdQuery, MatriculaDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMatriculaByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MatriculaDto> Handle(GetMatriculaByIdQuery request, CancellationToken cancellationToken)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(request.Id, cancellationToken);

            if (matricula == null)
                throw new KeyNotFoundException($"Matrícula com ID {request.Id} não encontrada");

            return MapToDto(matricula);
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
