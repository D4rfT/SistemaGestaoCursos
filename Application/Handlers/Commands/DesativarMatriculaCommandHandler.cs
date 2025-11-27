using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;


namespace Application.Handlers.Commands
{
    public class DesativarMatriculaCommandHandler : IRequestHandler<DesativarMatriculaCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesativarMatriculaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DesativarMatriculaCommand request, CancellationToken cancellationToken)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(request.Id, cancellationToken);
            if (matricula == null)
                throw new InvalidOperationException($"Não existe nenhuma matrícula com o id'{request.Id}'");

            if (!matricula.Ativa)
                throw new InvalidOperationException($"A matrícula de id {matricula.Id} já está desativada.");

            matricula.CancelarMatricula();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
}
