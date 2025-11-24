using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Handlers.Commands
{
    public class DesativarCursoCommandHandler : IRequestHandler<DesativarCursoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesativarCursoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DesativarCursoCommand request, CancellationToken cancellationToken)
        {
            var curso = await _unitOfWork.Cursos.GetByIdAsync(request.Id, cancellationToken);
            if (curso == null)
                throw new InvalidOperationException($"Não existe nenhum curso com o id'{request.Id}'");

            if (!curso.Ativo)
                throw new InvalidOperationException($"O Curso {curso.Nome} já está desativado.");

            curso.Desativar();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
