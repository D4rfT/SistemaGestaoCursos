using Application.Interfaces;
using Application.Models;
using Application.Queries;
using Domain.Entities;
using MediatR;

namespace Application.Handlers.Queries
{
    public class GetMatriculasPorAlunoQueryHandler : IRequestHandler<GetMatriculasPorAlunoQuery, List<MatriculaDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMatriculasPorAlunoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MatriculaDto>> Handle(GetMatriculasPorAlunoQuery request, CancellationToken cancellationToken)
        {
            var matriculas = await _unitOfWork.Matriculas.GetMatriculasPorAlunoAsync(request.Id, cancellationToken);

            return matriculas.Select(MapToDto).ToList();    
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
