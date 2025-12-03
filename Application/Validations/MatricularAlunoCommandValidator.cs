using Application.Commands;
using FluentValidation;


namespace Application.Validations
{
    public class MatricularAlunoCommandValidator : AbstractValidator<MatricularAlunoCommand>
    {
        public MatricularAlunoCommandValidator()
        {
            RuleFor(x => x.AlunoId)
                .NotEmpty().WithMessage("ID do aluno é obrigatório")
                .GreaterThan(0).WithMessage("ID do aluno deve ser maior que zero");

            RuleFor(x => x.CursoId)
                .NotEmpty().WithMessage("ID do curso é obrigatório")
                .GreaterThan(0).WithMessage("ID do curso deve ser maior que zero");

        }
    }
}
