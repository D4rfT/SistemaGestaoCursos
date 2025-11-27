using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Handlers.Commands
{
    public class DesativarAlunoCommandHandler : IRequestHandler<DesativarAlunoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DesativarAlunoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DesativarAlunoCommand request, CancellationToken cancellationToken)
        {
            var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
            if (aluno == null)
                throw new InvalidOperationException($"Não existe Aluno com o id {request.Id}");

            if (!aluno.Ativo)
                throw new InvalidOperationException($"O Aluno de id {request.Id} já está desativado");

            aluno.DesativarAluno();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;

        }
    }
}
