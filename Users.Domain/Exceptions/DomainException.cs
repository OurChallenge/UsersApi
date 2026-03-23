namespace Users.Domain.Exceptions;

public class DomainException : Exception
{
    public string? Code { get; }

    public IDictionary<string, string[]>? Errors { get; }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, string? code = null)
        : base(message)
    {
        Code = code;
    }

    public DomainException(string message, IDictionary<string, string[]> errors, string? code = null)
        : base(message)
    {
        Errors = errors;
        Code = code;
    }
}
