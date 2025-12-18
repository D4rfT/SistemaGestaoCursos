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
    public class DesativarAlunoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComAlunoExistenteEAtivo_DeveDesativar()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<DesativarAlunoCommandHandler>>();
            var handler = new DesativarAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344", "email@teste.com",new DateTime(2000, 1, 1), 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            var result = await handler.Handle(new DesativarAlunoCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            aluno.Ativo.Should().BeFalse();
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ComAlunoNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<DesativarAlunoCommandHandler>>();
            var handler = new DesativarAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new DesativarAlunoCommand(1), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComAlunoJaDesativado_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<DesativarAlunoCommandHandler>>();
            var handler = new DesativarAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344", "email@teste.com",
                new DateTime(2000, 1, 1), 1);
            aluno.DesativarAluno();
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new DesativarAlunoCommand(1), CancellationToken.None));
        }
    }
}