using Application.Commands;
using Application.Handlers.Commands;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UnitTests.Application.Handlers.Commands
{
    public class CreateCursoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DadosValidos_DeveCriarCurso()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 40
            };

            //reflection para setar Id
            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var idProperty = typeof(Domain.Common.BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Curso Teste");
        }

        [Fact]
        public async Task Handle_ComNomeDuplicado_DeveLancarExcecao()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Existente",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 40
            };

            // Mock retorna curso existente (nome duplicado)
            var cursoExistente = new Curso("Curso Teste", "Descrição", 1000, 40);
            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync("Curso Existente", It.IsAny<CancellationToken>())).ReturnsAsync(cursoExistente);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

        }
        [Fact]
        public async Task Handle_ComPrecoNegativo_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = -1000,
                Duracao = 40
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Curso)null);

            //act + assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

        }

        [Fact]
        public async Task Handle_ComDuracaoZero_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 0
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Curso)null);

            //act + assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComDuracaoAcimaDe500_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 501
            };

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Domain.Entities.Curso)null);

            //act + assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComDuracaoMinima_DeveCriarCurso()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 1
            };

            var curso = new Curso("Curso Teste", "Descrição", 1000, 1);
            var idProperty = typeof(Domain.Common.BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Curso Teste");
        }

        [Fact]
        public async Task Handle_ComDuracaoMaxima_DeveCriarCurso()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 1000,
                Duracao = 500
            };

            var curso = new Curso("Curso Teste", "Descrição", 1000, 500);
            var idProperty = typeof(Domain.Common.BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Curso Teste");
        }

        [Fact]
        public async Task Handle_ComPrecoMinimo_DeveCriarCurso()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 0.01m,
                Duracao = 250
            };

            var curso = new Curso("Curso Teste", "Descrição", 0.01m, 250);
            var idProperty = typeof(Domain.Common.BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Curso Teste");
        }

        [Fact]
        public async Task Handle_ComPrecoMaximo_DeveCriarCurso()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateCursoCommandHandler>>();

            var handler = new CreateCursoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "Descrição",
                Preco = 10000,
                Duracao = 250
            };

            var curso = new Curso("Curso Teste", "Descrição", 10000, 250);
            var idProperty = typeof(Domain.Common.BaseEntity).GetProperty("Id");
            idProperty?.SetValue(curso, 1);

            unitOfWorkMock.Setup(u => u.Cursos.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be("Curso Teste");
        }
    }
 }