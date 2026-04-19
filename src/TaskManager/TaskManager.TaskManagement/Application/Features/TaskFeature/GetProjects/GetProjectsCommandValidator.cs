using FluentValidation;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetProjects;

public class GetProjectsCommandValidator : AbstractValidator<GetProjectsCommand>
{
    public GetProjectsCommandValidator()
    {
        RuleFor(x => x.Search)
            .NotNull().WithMessage("Search обязателен");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page не может быть меньше 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("PageSize должен быть от 1 до 50");
    }
}
