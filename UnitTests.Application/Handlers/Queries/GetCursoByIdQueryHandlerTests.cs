using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetCursoByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComCursoExistente_DeveRetornarCurso()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetCursoByIdQueryHandler>>();
            var handler = new GetCursoByIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock
                .Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(curso);

            var result = await handler.Handle(new GetCursoByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Nome.Should().Be("Curso Teste");
        }

        [Fact]
        public async Task Handle_ComCursoNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetCursoByIdQueryHandler>>();
            var handler = new GetCursoByIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => handler.Handle(new GetCursoByIdQuery(1), CancellationToken.None));
        }
    }
}
