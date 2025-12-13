using Application.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Handlers.Commands
{
    public class AutenticarCommandHandler : IRequestHandler<AutenticarCommand, AutenticacaoResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticarCommandHandler> _logger;

        public AutenticarCommandHandler(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<AutenticarCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<AutenticacaoResultDto> Handle(AutenticarCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Autenticando usuário: Email={request.Email}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(request.Email, cancellationToken);

                if (usuario == null)
                {
                    _logger.LogWarning($"Usuário não encontrado: Email={request.Email}");
                    throw new UnauthorizedAccessException("Credenciais inválidas");
                }

                if (!usuario.ValidarSenha(request.Senha))
                {
                    _logger.LogWarning($"Senha inválida para usuário: Email={request.Email}, UsuarioId={usuario.Id}");
                    throw new UnauthorizedAccessException("Credenciais inválidas");
                }

                if (!usuario.Ativo)
                {
                    _logger.LogWarning($"Usuário inativo: Email={request.Email}, UsuarioId={usuario.Id}");
                    throw new InvalidOperationException("Usuário inativo");
                }

                var token = GerarToken(usuario);
                stopwatch.Stop();

                _logger.LogInformation($"Autenticação bem-sucedida: Email={request.Email}, Role={usuario.Role}, Tempo={stopwatch.ElapsedMilliseconds}ms");

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
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, $"Erro na autenticação: Email={request.Email}, Tempo={stopwatch.ElapsedMilliseconds}ms");
                throw;
            }
        }

        private string GerarToken(Usuario usuario)
        {
            _logger.LogDebug($"Gerando JWT para usuário: UsuarioId={usuario.Id}, Role={usuario.Role}");

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

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}