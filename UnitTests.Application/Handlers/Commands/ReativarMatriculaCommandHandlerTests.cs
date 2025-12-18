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
    public class ReativarMatriculaCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComMatriculaExistenteEInativa_DeveReativar()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<ReativarMatriculaCommandHandler>>();
            var handler = new ReativarMatriculaCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var matricula = new Matricula(1, 1);
            matricula.CancelarMatricula();
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(matricula, 1);

            unitOfWorkMock.Setup(u => u.Matriculas.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var result = await handler.Handle(new ReativarMatriculaCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            matricula.Ativa.Should().BeTrue();
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}