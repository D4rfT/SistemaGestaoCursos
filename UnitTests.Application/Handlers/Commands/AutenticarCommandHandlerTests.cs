using Application.Commands;
using Application.Handlers.Commands;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Commands
{
    public class AutenticarCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComCredenciaisValidas_DeveRetornarToken()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var loggerMock = new Mock<ILogger<AutenticarCommandHandler>>();

            var handler = new AutenticarCommandHandler(unitOfWorkMock.Object, configurationMock.Object, loggerMock.Object);

            var usuario = new Usuario("Usuário", "teste@email.com", "senha123", "Aluno");
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(usuario, 1);

            unitOfWorkMock.Setup(u => u.Usuarios.GetByEmailAsync("teste@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

            configurationMock.Setup(c => c["JwtSettings:Secret"]).Returns("minha-chave-secreta-sistemas-de-cursos-32-caracteres!");
            configurationMock.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("1440");
            configurationMock.Setup(c => c["JwtSettings:Issuer"]).Returns("Teste");
            configurationMock.Setup(c => c["JwtSettings:Audience"]).Returns("Teste");

            var command = new AutenticarCommand("teste@email.com", "senha123");

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.NomeUsuario.Should().Be("Usuário");
            result.Role.Should().Be("Aluno");
        }

        [Fact]
        public async Task Handle_ComUsuarioNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var loggerMock = new Mock<ILogger<AutenticarCommandHandler>>();

            var handler = new AutenticarCommandHandler(unitOfWorkMock.Object, configurationMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Usuarios.GetByEmailAsync("inexistente@email.com", It.IsAny<CancellationToken>())).ReturnsAsync((Usuario)null);

            var command = new AutenticarCommand("inexistente@email.com", "senha123");

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComSenhaInvalida_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            var loggerMock = new Mock<ILogger<AutenticarCommandHandler>>();

            var handler = new AutenticarCommandHandler(unitOfWorkMock.Object, configurationMock.Object, loggerMock.Object);

            var usuario = new Usuario("Usuário", "teste@email.com", "senhaCorreta", "Aluno");

            unitOfWorkMock.Setup(u => u.Usuarios.GetByEmailAsync("teste@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

            var command = new AutenticarCommand("teste@email.com", "senhaErrada");

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}