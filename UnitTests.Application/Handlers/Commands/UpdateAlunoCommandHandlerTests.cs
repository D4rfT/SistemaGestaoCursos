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
    public class UpdateAlunoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComDadosValidos_DeveAtualizarAluno()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<UpdateAlunoCommandHandler>>();
            var handler = new UpdateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno Antigo", "11122233344",
                "RA001", "antigo@email.com", new DateTime(2000, 1, 1), 1, 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            var curso = new Curso("Curso Novo", "Desc", 2000, 60);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, 2);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync("novo@email.com", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(curso);
            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            var command = new UpdateAlunoCommand
            {
                Id = 1,
                Nome = "Aluno Novo",
                Email = "novo@email.com",
                DataNascimento = new DateTime(2001, 1, 1),
                CursoId = 2
            };

            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Nome.Should().Be("Aluno Novo");
            result.Email.Should().Be("novo@email.com");
            result.CursoId.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ComEmailDuplicado_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<UpdateAlunoCommandHandler>>();
            var handler = new UpdateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344",
                "RA001", "atual@email.com", new DateTime(2000, 1, 1), 1, 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync("duplicado@email.com", 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var command = new UpdateAlunoCommand
            {
                Id = 1,
                Nome = "Aluno",
                Email = "duplicado@email.com",
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComCursoNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<UpdateAlunoCommandHandler>>();
            var handler = new UpdateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344",
                "RA001", "email@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync("novo@email.com", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Curso)null);

            var command = new UpdateAlunoCommand
            {
                Id = 1,
                Nome = "Aluno",
                Email = "novo@email.com",
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 999
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}
