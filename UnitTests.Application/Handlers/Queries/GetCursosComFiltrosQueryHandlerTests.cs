using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetCursosComFiltrosQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComFiltroPorNome_DeveRetornarCursosFiltrados()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetCursosComFiltrosQueryHandler>>();
            var handler = new GetCursosComFiltrosQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var cursos = new List<Curso>
            {
                new Curso("C# Avançado", "Descrição", 1000, 40),
                new Curso("Java Básico", "Descrição", 800, 30)
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetCursosComFiltrosAsync("C#", null, null, null, null, null, null, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursos.Where(c => c.Nome.Contains("C#")).ToList());

            var query = new GetCursosComFiltrosQuery("C#", null, null, null, null, null, null, false);
            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Nome.Should().Be("C# Avançado");
        }

        [Fact]
        public async Task Handle_ComFiltroPorPreco_DeveRetornarCursosFiltrados()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetCursosComFiltrosQueryHandler>>();
            var handler = new GetCursosComFiltrosQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var cursos = new List<Curso>
            {
                new Curso("Curso 1", "Descrição", 500, 20),
                new Curso("Curso 2", "Descrição", 1500, 40)
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetCursosComFiltrosAsync(null, 1000m, null, null, null, null, null, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursos.Where(c => c.Preco >= 1000).ToList());

            var query = new GetCursosComFiltrosQuery(null, 1000m, null, null, null, null, null, false);
            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Preco.Should().Be(1500);
        }

        [Fact]
        public async Task Handle_SemFiltros_DeveRetornarTodosCursos()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetCursosComFiltrosQueryHandler>>();
            var handler = new GetCursosComFiltrosQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var cursos = new List<Curso>
            {
                new Curso("Curso 1", "Descrição", 500, 20),
                new Curso("Curso 2", "Descrição", 1500, 40)
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetCursosComFiltrosAsync(null, null, null, null, null, null, null, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cursos);

            var query = new GetCursosComFiltrosQuery();
            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}