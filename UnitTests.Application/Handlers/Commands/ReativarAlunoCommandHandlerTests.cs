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
    public class ReativarAlunoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComAlunoExistenteEInativo_DeveReativar()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<ReativarAlunoCommandHandler>>();
            var handler = new ReativarAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344", "email@teste.com",new DateTime(2000, 1, 1), 1);
            aluno.DesativarAluno();
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            var result = await handler.Handle(new ReativarAlunoCommand(1), CancellationToken.None);

            result.Should().BeTrue();
            aluno.Ativo.Should().BeTrue();
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ComAlunoJaAtivo_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<ReativarAlunoCommandHandler>>();
            var handler = new ReativarAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344", "email@teste.com",
                new DateTime(2000, 1, 1), 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new ReativarAlunoCommand(1), CancellationToken.None));
        }
    }
}