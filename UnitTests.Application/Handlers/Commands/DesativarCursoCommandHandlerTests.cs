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
    public class DesativarCursoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComCursoExistenteEAtivo_DeveDesativar()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<DesativarCursoCommandHandler>>();
            var handler = new DesativarCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var curso = new Curso("Curso", "Descrição", 1000, 40);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            var result = await handler.Handle(new DesativarCursoCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            curso.Ativo.Should().BeFalse();
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ComCursoJaDesativado_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<DesativarCursoCommandHandler>>();
            var handler = new DesativarCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var curso = new Curso("Curso", "Descrição", 1000, 40);
            curso.Desativar();
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new DesativarCursoCommand(1), CancellationToken.None));
        }
    }
}