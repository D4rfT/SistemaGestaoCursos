using Application.Commands;
using Application.Handlers.Commands;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Commands
{
    public class CreateAlunoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DadosValidos_DeveCriarAluno()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "11123456789",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1).ToUniversalTime(),
                CursoId = 1
            };

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, command.CursoId);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            var usuarioId = 1;
            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario usuario, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(usuario, usuarioId);
                    return usuario;
                });


            var aluno = new Aluno(
                command.Nome,
                command.CPF,
                "RA20250101000001",
                command.Email,
                command.DataNascimento,
                command.CursoId,
                usuarioId);

            var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
            alunoIdProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(command.Nome);
            result.CPF.Should().Be(command.CPF);
            result.Email.Should().Be(command.Email);
            result.CursoId.Should().Be(command.CursoId);
            result.Ativo.Should().BeTrue();

            unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            unitOfWorkMock.Verify(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Alunos.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_ComCpfExistente_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "11123456789",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1).ToUniversalTime(),
                CursoId = 1
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));


            unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ComEmailExistente_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "11111111111",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1).ToUniversalTime(),
                CursoId = 1
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

            unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ComCursoNaoExistente_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "11123456789",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1).ToUniversalTime(),
                CursoId = 2
            };

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, command.CursoId);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));

            unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ComMenorDe16Anos_DeveLancarExcecao()
        {
            //Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "11123456789",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2010, 1, 1).ToUniversalTime(),
                CursoId = 2
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, command.CursoId);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario usuario, CancellationToken ct) =>
            {
                var idProperty = typeof(BaseEntity).GetProperty("Id");
                idProperty?.SetValue(usuario, 1);
                return usuario;
            });

            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            //act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

            unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }
    }
}
