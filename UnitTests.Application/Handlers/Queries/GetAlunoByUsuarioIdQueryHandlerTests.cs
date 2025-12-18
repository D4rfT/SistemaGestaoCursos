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
    public class GetAlunoByUsuarioIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComUsuarioComAluno_DeveRetornarAluno()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAlunoByUsuarioIdQueryHandler>>();
            var handler = new GetAlunoByUsuarioIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno", "11122233344","RA001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 100);

            var alunosList = new List<Aluno> { aluno };

            unitOfWorkMock.Setup(u => u.Alunos.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Aluno, bool>>>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(alunosList);

            var result = await handler.Handle(new GetAlunoByUsuarioIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(100);
            result.Nome.Should().Be("Aluno");
        }

        [Fact]
        public async Task Handle_ComUsuarioSemAluno_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAlunoByUsuarioIdQueryHandler>>();
            var handler = new GetAlunoByUsuarioIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Alunos.FindAsync(a => a.UsuarioId == 1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Aluno>());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new GetAlunoByUsuarioIdQuery(1), CancellationToken.None));
        }
    }
}