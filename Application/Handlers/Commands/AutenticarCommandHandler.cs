using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Handlers.Commands
{
    public class AutenticarCommandHandler : IRequestHandler<AutenticarCommand, AutenticacaoResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AutenticarCommandHandler(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AutenticacaoResultDto> Handle(AutenticarCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email, cancellationToken);

            if (usuario == null || !usuario.ValidarSenha(request.Senha))
                throw new UnauthorizedAccessException("Credenciais inválidas");

            if (!usuario.Ativo)
                throw new InvalidOperationException("Usuário inativo");

            var token = GerarToken(usuario);

            return new AutenticacaoResultDto
            {
                Token = token,
                ExpiraEm = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"])),
                NomeUsuario = usuario.Nome,
                Email = usuario.Email,
                Role = usuario.Role
            };
        }

        private string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:ExpirationInMinutes"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}