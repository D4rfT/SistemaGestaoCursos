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
    public class UpdateCursoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarCurso()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<UpdateCursoCommandHandler>>();
            var handler = new UpdateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var curso = new Curso("Curso Antigo", "Desc Antiga", 1000, 40);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock
                .Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(curso);

            unitOfWorkMock
                .Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(curso);

            var command = new UpdateCursoCommand
            {
                Id = 1,
                Nome = "Curso Novo",
                Descricao = "Descrição Nova",
                Preco = 2000,
                Duracao = 60
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Nome.Should().Be("Curso Novo");
            result.Preco.Should().Be(2000);
            result.Duracao.Should().Be(60);
        }

        [Fact]
        public async Task Handle_ComCursoNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<UpdateCursoCommandHandler>>();
            var handler = new UpdateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);

            var command = new UpdateCursoCommand
            {
                Id = 999,
                Nome = "Curso",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 40
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(command, CancellationToken.None));
        }
    }
}

