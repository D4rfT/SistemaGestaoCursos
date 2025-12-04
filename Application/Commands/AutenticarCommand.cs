using Application.Models;
using MediatR;

namespace Application.Commands
{
    public class AutenticarCommand : IRequest<AutenticacaoResultDto>
    {
        public string Email { get; set; }
        public string Senha { get; set; }

        public AutenticarCommand(string email, string senha)
        {
            Email = email;
            Senha = senha;
        }
    }
}