using Application.Commands;
using FluentValidation;

namespace Application.Validations
{
    public class UpdateAlunoCommandValidator : AbstractValidator<UpdateAlunoCommand>
    {
        public UpdateAlunoCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter pelo menos 3 caracteres")
                .MaximumLength(100).WithMessage("Nome não pode exceder 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e espaços");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(100).WithMessage("Email não pode exceder 100 caracteres");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("Data de nascimento é obrigatória")
                .LessThan(DateTime.Now).WithMessage("Data de nascimento não pode ser futura")
                .Must(SerMaiorDe16Anos).WithMessage("Aluno deve ter pelo menos 16 anos");

            RuleFor(x => x.CursoId)
                .NotNull().WithMessage("ID do curso é obrigatório")
                .GreaterThan(0).WithMessage("ID do curso inválido");
        }

        private bool SerMaiorDe16Anos(DateTime dataNascimento)
        {
            var idade = DateTime.Now.Year - dataNascimento.Year;

            //aniversário
            if (DateTime.Now < dataNascimento.AddYears(idade))
                idade--;

            return idade >= 16;
        }
    }
}
