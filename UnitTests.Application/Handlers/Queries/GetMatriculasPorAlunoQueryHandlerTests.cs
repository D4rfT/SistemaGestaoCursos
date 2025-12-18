using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetMatriculasPorAlunoQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComAlunoExistente_DeveRetornarMatriculas()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetMatriculasPorAlunoQueryHandler>>();
            var handler = new GetMatriculasPorAlunoQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var matriculas = new List<Domain.Entities.Matricula>
            {
                new Domain.Entities.Matricula(1, 1),
                new Domain.Entities.Matricula(1, 2)
            };

            unitOfWorkMock.Setup(u => u.Matriculas.GetMatriculasPorAlunoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(matriculas);

            var result = await handler.Handle(new GetMatriculasPorAlunoQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ComAlunoSemMatriculas_DeveRetornarListaVazia()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetMatriculasPorAlunoQueryHandler>>();
            var handler = new GetMatriculasPorAlunoQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Matriculas.GetMatriculasPorAlunoAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Domain.Entities.Matricula>());

            var result = await handler.Handle(new GetMatriculasPorAlunoQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}