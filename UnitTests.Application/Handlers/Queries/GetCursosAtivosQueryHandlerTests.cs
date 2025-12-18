using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetCursosAtivosQueryHandlerTests
    {
        [Fact]
        public async Task Handle_DeveRetornarApenasCursosAtivos()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetCursosAtivosQueryHandler>>();
            var handler = new GetCursosAtivosQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var cursoAtivo = new Curso("Curso Ativo", "Descrição", 1000, 40);

            var cursoInativo = new Curso("Curso Inativo", "Descrição", 1000, 40);
            cursoInativo.Desativar();

            var cursos = new List<Curso> { cursoAtivo, cursoInativo };

            unitOfWorkMock.Setup(u => u.Cursos.GetCursosAtivosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(cursos.Where(c => c.Ativo).ToList());

            var result = await handler.Handle(new GetCursosAtivosQuery(), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Nome.Should().Be("Curso Ativo");
        }
    }
}