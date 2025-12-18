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
    public class MatricularAlunoCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ComDadosValidos_DeveMatricularAluno()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MatricularAlunoCommandHandler>>();
            var handler = new MatricularAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344","RA001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
            alunoIdProperty?.SetValue(aluno, 1);

            var curso = new Curso("Curso", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, 1);

            var matricula = new Matricula(1, 1);
            var matriculaIdProperty = typeof(BaseEntity).GetProperty("Id");
            matriculaIdProperty?.SetValue(matricula, 100);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(curso);
            unitOfWorkMock.Setup(u => u.Matriculas.ExisteMatriculaAtivaAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            unitOfWorkMock.Setup(u => u.Matriculas.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>())).ReturnsAsync((Matricula m, CancellationToken ct) => m);

            unitOfWorkMock.Setup(u => u.Matriculas.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var command = new MatricularAlunoCommand(1, 1);
            var result = await handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.AlunoId.Should().Be(1);
            result.CursoId.Should().Be(1);
            result.Ativa.Should().BeTrue();

            unitOfWorkMock.Verify(u => u.Matriculas.AddAsync(It.IsAny<Matricula>(), It.IsAny<CancellationToken>()), Times.Once);
            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ComAlunoNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MatricularAlunoCommandHandler>>();
            var handler = new MatricularAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())) .ReturnsAsync((Aluno)null);

            var command = new MatricularAlunoCommand(1, 1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComAlunoInativo_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MatricularAlunoCommandHandler>>();
            var handler = new MatricularAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344","RA001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            aluno.DesativarAluno();

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            var command = new MatricularAlunoCommand(1, 1);
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComCursoInativo_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MatricularAlunoCommandHandler>>();
            var handler = new MatricularAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344","RA001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var curso = new Curso("Curso", "Descrição", 1000, 40);
            curso.Desativar();

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(curso);

            var command = new MatricularAlunoCommand(1, 1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComMatriculaDuplicada_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MatricularAlunoCommandHandler>>();
            var handler = new MatricularAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344","RA001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var curso = new Curso("Curso", "Descrição", 1000, 40);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(curso);
            unitOfWorkMock.Setup(u => u.Matriculas.ExisteMatriculaAtivaAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var command = new MatricularAlunoCommand(1, 1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}