using Application.Handlers.Queries;
using Application.Interfaces;
using Application.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Application.Handlers.Queries
{
    public class GetAllAlunosQueryHandlerTests
    {
        [Fact]
        public async Task Handle_DeveRetornarTodosAlunos()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<GetAllAlunosQueryHandler>>();
            var handler = new GetAllAlunosQueryHandler(unitOfWorkMock.Object, loggerMock.Object);

            var alunos = new List<Aluno>
            {
                new Aluno("Aluno 1", "11111111111", "RA001", "aluno1@teste.com", new DateTime(2000, 1, 1), 1, 1),
                new Aluno("Aluno 2", "22222222222", "RA002", "aluno2@teste.com", new DateTime(2001, 1, 1), 2, 2)
            };

            unitOfWorkMock.Setup(u => u.Alunos.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(alunos);

            var result = await handler.Handle(new GetAllAlunosQuery(), CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}