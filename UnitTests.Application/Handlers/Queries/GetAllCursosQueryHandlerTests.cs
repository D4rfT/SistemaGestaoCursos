using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;


namespace UnitTests.Application.Handlers.Queries
{
    public class GetAllCursosQueryHandlerTests
    {
        [Fact]
        public async Task Handle_DeveRetornarTodosCursos()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAllCursosQueryHandler>>();
            var handler = new GetAllCursosQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var cursos = new List<Curso>
            {
                new Curso("Curso 1", "Descrição 1", 1000, 40),
                new Curso("Curso 2", "Descrição 2", 2000, 60)
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(cursos);

            var result = await handler.Handle(new GetAllCursosQuery(), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}