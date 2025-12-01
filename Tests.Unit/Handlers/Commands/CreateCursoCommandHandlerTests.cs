using Application.Commands;
using Application.Handlers.Commands;
using Application.Interfaces;
using Application.Models;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Tests.Unit.Handlers.Commands
{
    public class CreateCursoCommandHandlerTests
    {
        // Mocks globais
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICursoRepository> _mockCursoRepository;
        private readonly CreateCursoCommandHandler _handler;

        public CreateCursoCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCursoRepository = new Mock<ICursoRepository>();

            // Configurar UnitOfWork para retornar o mock do repositório
            _mockUnitOfWork.SetupGet(u => u.Cursos).Returns(_mockCursoRepository.Object);

            _handler = new CreateCursoCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_CursoValido_DeveCriarCursoERetornarDto()
        {
            // ARRANGE
            var command = new CreateCursoCommand("C# Avançado", "Curso completo de C#", 299.90m, 40);

            // Configurar mock para NÃO encontrar curso com mesmo nome
            _mockCursoRepository.Setup(r => r.GetByNomeAsync("C# Avançado", It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Curso)null);

            // Configurar mock para "salvar" o curso
            _mockCursoRepository.Setup(r => r.AddAsync(It.IsAny<Curso>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Curso curso) => curso);

            // Configurar mock para retornar curso com ID após salvar
            _mockCursoRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((int id) => new Curso("C# Avançado", "Curso completo de C#", 299.90m, 40) { Id = id });

            // ACT
            var result = await _handler.Handle(command, CancellationToken.None);

            // ASSERT
            result.Should().NotBeNull();
            result.Nome.Should().Be("C# Avançado");
            result.Descricao.Should().Be("Curso completo de C#");
            result.Preco.Should().Be(299.90m);
            result.Duracao.Should().Be(40);
            result.Ativo.Should().BeTrue();

            // Verificar se os métodos foram chamados
            _mockCursoRepository.Verify(r => r.GetByNomeAsync("C# Avançado", It.IsAny<CancellationToken>()), Times.Once);
            _mockCursoRepository.Verify(r => r.AddAsync(It.IsAny<Curso>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CursoComNomeExistente_DeveLancarInvalidOperationException()
        {
            // ARRANGE
            var command = new CreateCursoCommand("C#", "Novo curso", 100, 30);
            var cursoExistente = new Curso("C#", "Curso existente", 200, 60);

            _mockCursoRepository.Setup(r => r.GetByNomeAsync("C#", It.IsAny<CancellationToken>()))
                               .ReturnsAsync(cursoExistente);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain("Já existe um curso com o nome 'C#'");

            // Verificar que AddAsync NÃO foi chamado
            _mockCursoRepository.Verify(r => r.AddAsync(It.IsAny<Curso>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComandoNulo_DeveLancarArgumentNullException()
        {
            // ARRANGE
            CreateCursoCommand command = null;

            // ACT & ASSERT
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Theory]
        [InlineData("", "Descrição", 100, 40, "nome")]
        [InlineData("Curso", "", 100, 40, "descricao")]
        [InlineData("Curso", "Descrição", -10, 40, "preco")]
        [InlineData("Curso", "Descrição", 100, 0, "duracao")]
        public async Task Handle_DadosInvalidos_EntidadeDeveLancarExcecao(
            string nome, string descricao, decimal preco, int duracao, string campoInvalido)
        {
            // ARRANGE
            var command = new CreateCursoCommand(nome, descricao, preco, duracao);

            _mockCursoRepository.Setup(r => r.GetByNomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Curso)null);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));

            exception.Message.Should().Contain(campoInvalido, StringComparison.OrdinalIgnoreCase);
        }
    }
}