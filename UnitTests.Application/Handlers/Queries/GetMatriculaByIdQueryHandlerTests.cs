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
    public class GetMatriculaByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComMatriculaExistente_DeveRetornarMatricula()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetMatriculaByIdQueryHandler>>();
            var handler = new GetMatriculaByIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344",
                "RA001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var curso = new Curso("Curso", "Desc", 1000, 40);

            var matricula = new Matricula(1, 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(matricula, 1);

            unitOfWorkMock.Setup(u => u.Matriculas.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var result = await handler.Handle(new GetMatriculaByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.AlunoId.Should().Be(1);
            result.CursoId.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ComMatriculaNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetMatriculaByIdQueryHandler>>();
            var handler = new GetMatriculaByIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Matriculas.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Matricula)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new GetMatriculaByIdQuery(1), CancellationToken.None));
        }
    }
}