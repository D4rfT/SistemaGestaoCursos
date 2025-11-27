using Application.Commands;
using Application.Interfaces;
using Application.Models;
using MediatR;

namespace Application.Handlers.Commands
{
    public class ReativarAlunoCommandHandler : IRequestHandler<ReativarAlunoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReativarAlunoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ReativarAlunoCommand request, CancellationToken cancellationToken)
        {
            var aluno = await _unitOfWork.Alunos.GetByIdAsync(request.Id, cancellationToken);
            if (aluno == null)
                throw new InvalidOperationException($"Não existe Aluno com o id {request.Id}");

            if (aluno.Ativo)
                throw new InvalidOperationException($"O Aluno do id {request.Id} já está ativo");

            aluno.ReativarAluno();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
