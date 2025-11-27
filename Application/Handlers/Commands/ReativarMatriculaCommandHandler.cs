using Application.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Handlers.Commands
{
    public class ReativarMatriculaCommandHandler : IRequestHandler<ReativarMatriculaCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReativarMatriculaCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ReativarMatriculaCommand request,  CancellationToken cancellationToken)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(request.Id, cancellationToken);
            if (matricula == null)
                throw new InvalidOperationException($"Não existe nenhuma matrícula com o id'{request.Id}'");

            if (matricula.Ativa)
                throw new InvalidOperationException($"A matrícula do id {request.Id} já está ativa.");

            matricula.ReativarMatricula();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

    }
}
