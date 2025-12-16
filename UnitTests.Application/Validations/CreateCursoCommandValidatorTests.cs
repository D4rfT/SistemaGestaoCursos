using Application.Commands;
using Application.Validations;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Application.Validations
{
    public class CreateCursoCommandValidatorTests
    {
        private readonly CreateCursoCommandValidator _validator;

        public CreateCursoCommandValidatorTests()
        {
            _validator = new CreateCursoCommandValidator();
        }

        [Fact]
        public void Validator_ComNomeVazio_DeveTerErro()
        {
            // Arrange
            var command = new CreateCursoCommand
            {
                Nome = "",
                Descricao = "Descrição válida",
                Preco = 1000,
                Duracao = 40
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.Nome);
        }

        [Fact]
        public async Task Handle_ComDescricaoCurta_DeveLancarValidationException()
        {
            var command = new CreateCursoCommand
            {
                Nome = "Curso Teste",
                Descricao = "DesCurta",
                Preco = 1000,
                Duracao = 40
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.Descricao);
        }
    }
}