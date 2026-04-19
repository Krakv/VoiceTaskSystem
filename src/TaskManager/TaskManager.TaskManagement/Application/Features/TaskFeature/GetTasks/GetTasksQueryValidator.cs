using FluentValidation;

namespace TaskManager.TaskManagement.Application.Features.TaskFeature.GetTasks;

public class GetTasksQueryValidator
    : AbstractValidator<GetTasksQuery>
{
    public GetTasksQueryValidator()
    {
        RuleFor(x => x.Query)
            .MaximumLength(200)
            .When(x => x.Query != null);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Page не может быть меньше 0");

        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Limit должен быть от 1 до 100");

        RuleFor(x => x.SortOrder)
            .Must(x => x == null || x == "ASC" || x == "DESC")
            .WithMessage("SortOrder должен быть ASC или DESC");

        RuleFor(x => x.SortBy)
            .Must(BeAllowedSortColumn)
            .When(x => x.SortBy != null)
            .WithMessage("Недопустимое поле сортировки");
    }

    private static bool BeAllowedSortColumn(string? column)
    {
        if (string.IsNullOrWhiteSpace(column))
            return true;

        return column is "DueDate" or "CreatedAt" or "Priority" or "Status" or "Title";
    }
}
