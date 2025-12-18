using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetMatriculasPorCursoQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComCursoExistente_DeveRetornarMatriculas()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetMatriculasPorCursoQueryHandler>>();
            var handler = new GetMatriculasPorCursoQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var matriculas = new List<Matricula>
            {
                new Matricula(1, 1),
                new Matricula(2, 1)
            };

            unitOfWorkMock.Setup(u => u.Matriculas.GetMatriculasPorCursoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(matriculas);

            var result = await handler.Handle(new GetMatriculasPorCursoQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}