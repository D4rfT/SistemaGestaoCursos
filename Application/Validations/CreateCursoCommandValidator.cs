using Application.Commands;
using FluentValidation;

namespace Application.Validations
{
    public class CreateCursoCommandValidator :AbstractValidator<CreateCursoCommand>
    {
        public CreateCursoCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do curso é obrigatório")
                .MinimumLength(3).WithMessage("O nome do curso deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("O nome do curso deve ter no máximo 100 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome contém caracteres inválidos");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("A descrição do curso é obrigatória")
                .MinimumLength(10).WithMessage("A descrição do curso deve ter no mínimo 10 caracteres")
                .MaximumLength(500).WithMessage("A descrição do curso deve ter no máximo 500 caracteres");

            RuleFor(x => x.Preco)
                .NotEmpty().WithMessage("O preço do curso é obrigatório")
                .GreaterThan(0).WithMessage("O preço do curso deve ser maior que zero")
                .LessThanOrEqualTo(10000).WithMessage("O preço não pode exceder R$ 10.000,00")
                .PrecisionScale(10, 2, ignoreTrailingZeros: true).WithMessage("Preço deve ter no máximo 2 casas decimais");

            RuleFor(x => x.Duracao)
                .NotEmpty().WithMessage("A duração do curso é obrigatória")
                .GreaterThan(0).WithMessage("A duração do curso deve ser maior que zero")
                .LessThanOrEqualTo(500).WithMessage("Duração não pode exceder 500 horas"); 


        }
    }
}
