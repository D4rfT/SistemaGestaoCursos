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

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, command.CursoId);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(curso);
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            var usuarioId = 1;
            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario usuario, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(usuario, usuarioId);
                    return usuario;
                });

            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

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
        public async Task Handle_DadosValidos_DeveCriarAlunoComUsuario()
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

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, command.CursoId);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(curso);
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            var usuarioId = 1;
            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario usuario, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(usuario, usuarioId);
                    return usuario;
                });

            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var aluno = new Aluno(
                command.Nome,
                command.CPF,
                "RA20250101000001",
                command.Email,
                command.DataNascimento,
                command.CursoId,
                usuarioId);

            var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
            alunoIdProperty?.SetValue(aluno, 100);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

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
            unitOfWorkMock.Verify(u => u.Usuarios.AddAsync(It.Is<Usuario>(u =>
                u.Nome == command.Nome &&
                u.Email == command.Email &&
                u.SenhaHash == command.CPF &&
                u.Role == "Aluno"),
                It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(u => u.Alunos.AddAsync(It.Is<Aluno>(a =>
                a.Nome == command.Nome &&
                a.CPF == command.CPF &&
                a.Email == command.Email &&
                a.CursoId == command.CursoId &&
                a.UsuarioId == usuarioId),
                It.IsAny<CancellationToken>()), Times.Once);

            unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
            unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);


        }
        [Fact]
        public async Task Handle_DeveGerarRegistroAcademicoUnico()
        {
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

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var curso = new Curso("Curso Teste", "Descrição", 1000, 40);
            var cursoIdProperty = typeof(BaseEntity).GetProperty("Id");
            cursoIdProperty?.SetValue(curso, command.CursoId);

            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(curso);
            var raGerados = new List<string>();
            var tentativa = 0;

            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string ra, CancellationToken ct) =>
                {
                    tentativa++;
                    if (tentativa == 1)
                    {
                        var alunoExistente = new Aluno("Existente", "99999999999", ra, "existente@email.com",
                            new DateTime(1990, 1, 1), 1, 999);
                        return alunoExistente;
                    }
                    return null;
                });

            var usuarioId = 1;
            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario usuario, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(usuario, usuarioId);
                    return usuario;
                });


            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int id, CancellationToken ct) =>
                {
                    var aluno = new Aluno(command.Nome, command.CPF, "RA_UNICO", command.Email,command.DataNascimento, command.CursoId, usuarioId);
                    var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
                    alunoIdProperty?.SetValue(aluno, id);
                    return aluno;
                });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.RegistroAcademico.Should().NotBeNullOrEmpty();

            unitOfWorkMock.Verify(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),Times.AtLeast(2));

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

        [Fact]
        public async Task Handle_ComIdadeExatamente16Anos_DeveCriarAluno()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var dataNascimento = DateTime.UtcNow.AddYears(-16);
            var command = new CreateAlunoCommand
            {
                Nome = "Aluno 16 Anos",
                CPF = "11123456789",
                Email = "aluno16@email.com",
                DataNascimento = dataNascimento,
                CursoId = 1
            };

            SetupMocksParaSucesso(unitOfWorkMock, command);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.DataNascimento.Should().Be(command.DataNascimento);
        }

        [Fact]
        public async Task Handle_ComErroAoSavarUsuario_DeveLancarExcecaoERollback()
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
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new Curso("Curso", "Desc", 1000, 40));
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Erro ao salvar usuário"));

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(
                () => handler.Handle(command, CancellationToken.None));

            unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            unitOfWorkMock.Verify(u => u.Alunos.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComErroAoSavarAluno_DeveLancarExcecaoERollback()
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
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new Curso("Curso", "Desc", 1000, 40));
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

 
            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Usuario u, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(u, 1);
                    return u;
                });

            unitOfWorkMock.SetupSequence(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1).ThrowsAsync(new Exception("Erro ao salvar aluno"));

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(
                () => handler.Handle(command, CancellationToken.None));

            unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
            unitOfWorkMock.Verify(u => u.Alunos.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ComErroAoGerarRA_DeveLancarExcecao()
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
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new Curso("Curso", "Desc", 1000, 40));


            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string ra, CancellationToken ct) =>
                {
                    var aluno = new Aluno("Existente", "99999999999", ra, "existente@email.com",
                        new DateTime(1990, 1, 1), 1, 999);
                    return aluno;
                });

            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario u, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(u, 1);
                    return u;
                });

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(command, CancellationToken.None));

            unitOfWorkMock.Verify(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeast(5));
        }

        [Fact]
        public async Task Handle_DeveAssociarUsuarioAoAluno()
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
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            SetupMocksParaSucesso(unitOfWorkMock, command);

            var usuarioIdEsperado = 1;

            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario usuario, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(usuario, usuarioIdEsperado);
                    return usuario;
                });

            Aluno alunoAdicionado = null;
            unitOfWorkMock.Setup(u => u.Alunos.AddAsync(It.IsAny<Aluno>(), It.IsAny<CancellationToken>()))
                .Callback((Aluno aluno, CancellationToken ct) => alunoAdicionado = aluno)
                .ReturnsAsync((Aluno aluno, CancellationToken ct) => aluno);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            alunoAdicionado.Should().NotBeNull();
            alunoAdicionado.UsuarioId.Should().Be(usuarioIdEsperado);
        }

        [Fact]
        public async Task Handle_ComCPFInvalido_DeveLancarArgumentException()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "123", // CPF inválido (menos de 11 dígitos)
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new Curso("Curso", "Desc", 1000, 40));
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario u, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(u, 1);
                    return u;
                });

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComEmailInvalido_DeveLancarArgumentException()
        {
            // Arrange
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<CreateAlunoCommandHandler>>();
            var handler = new CreateAlunoCommandHandler(unitOfWorkMock.Object, loggerMock.Object);

            var command = new CreateAlunoCommand
            {
                Nome = "Aluno Teste",
                CPF = "11123456789",
                Email = "email-invalido", // Email sem @
                DataNascimento = new DateTime(2000, 1, 1),
                CursoId = 1
            };

            SetupMocksBasicos(unitOfWorkMock, command);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ComDataNascimentoFutura_DeveLancarArgumentException()
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
                DataNascimento = DateTime.UtcNow.AddDays(1), // Data futura
                CursoId = 1
            };

            SetupMocksBasicos(unitOfWorkMock, command);

            // Act + Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }


        private void SetupMocksParaSucesso(Mock<IUnitOfWork> unitOfWorkMock, CreateAlunoCommand command)
        {
            SetupMocksBasicos(unitOfWorkMock, command);
            unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((int id, CancellationToken ct) =>
                {
                    var aluno = new Aluno(command.Nome, command.CPF, "RA123", command.Email,
                        command.DataNascimento, command.CursoId, 1);
                    var alunoIdProperty = typeof(BaseEntity).GetProperty("Id");
                    alunoIdProperty?.SetValue(aluno, id);
                    return aluno;
                });
        }

        private void SetupMocksBasicos(Mock<IUnitOfWork> unitOfWorkMock, CreateAlunoCommand command)
        {
            unitOfWorkMock.Setup(u => u.Alunos.VerificarCpfExistenteAsync(command.CPF, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Alunos.VerificarEmailExistenteAsync(command.Email, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            unitOfWorkMock.Setup(u => u.Cursos.GetByIdAsync(command.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new Curso("Curso", "Desc", 1000, 40));
            unitOfWorkMock.Setup(u => u.Alunos.GetByRegistroAcademicoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            unitOfWorkMock.Setup(u => u.Usuarios.AddAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario u, CancellationToken ct) =>
                {
                    var idProperty = typeof(BaseEntity).GetProperty("Id");
                    idProperty?.SetValue(u, 1);
                    return u;
                });
        }
    }
}
