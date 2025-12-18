using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetAllMatriculasQueryHandlerTests
    {
        [Fact]
        public async Task Handle_DeveRetornarTodasMatriculas()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAllMatriculasQueryHandler>>();
            var handler = new GetAllMatriculasQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var matriculas = new List<Matricula>
            {
                new Matricula(1, 1),
                new Matricula(2, 2)
            };

            unitOfWorkMock.Setup(u => u.Matriculas.GetMatriculasComDadosRelacionadosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(matriculas);

            var result = await handler.Handle(new GetAllMatriculasQuery(), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}