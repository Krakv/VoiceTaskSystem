using FluentValidation;
using FluentValidation.TestHelper;

namespace TaskManager.UnitTests.TaskManagement;

public abstract class ValidatorTestBase<TValidator, TModel>
    where TValidator : AbstractValidator<TModel>, new()
{
    protected readonly TValidator Validator = new();

    protected TestValidationResult<TModel> Validate(TModel model)
    {
        return Validator.TestValidate(model);
    }
}
