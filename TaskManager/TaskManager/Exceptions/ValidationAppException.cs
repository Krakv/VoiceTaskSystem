namespace TaskManager.Exceptions;

public class ValidationAppException : Exception
{
    public string ErrorCode { get; }
    public IDictionary<string, string>? Errors { get; }

    // Конструктор для одиночной ошибки
    public ValidationAppException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
        Errors = null;
    }

    // Конструктор для составной ошибки
    public ValidationAppException(string errorCode, IDictionary<string, string> errors)
        : base("Произошла одна или несколько ошибок валидации")
    {
        ErrorCode = errorCode;
        Errors = errors;
    }
}
