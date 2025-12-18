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
    public class GetAlunoByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ComAlunoExistente_DeveRetornarAluno()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAlunoByIdQueryHandler>>();
            var handler = new GetAlunoByIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var aluno = new Aluno("Aluno Teste", "11122233344", "RA20240001", "aluno@teste.com", new DateTime(2000, 1, 1), 1, 1);
            var idProperty = typeof(BaseEntity).GetProperty("Id");
            idProperty?.SetValue(aluno, 1);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            var result = await handler.Handle(new GetAlunoByIdQuery(1), CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Nome.Should().Be("Aluno Teste");
        }

        [Fact]
        public async Task Handle_ComAlunoNaoExistente_DeveLancarExcecao()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAlunoByIdQueryHandler>>();
            var handler = new GetAlunoByIdQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            unitOfWorkMock.Setup(u => u.Alunos.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Aluno)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(new GetAlunoByIdQuery(1), CancellationToken.None));
        }
    }
}