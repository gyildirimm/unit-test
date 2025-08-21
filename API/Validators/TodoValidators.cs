using API.DTOs;
using FluentValidation;

namespace API.Validators;

public class TodoCreateDtoValidator : AbstractValidator<TodoCreateDto>
{
    public TodoCreateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(200);
    }
}

public class TodoUpdateDtoValidator : AbstractValidator<TodoUpdateDto>
{
    public TodoUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(200);
    }
}

