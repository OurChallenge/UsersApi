namespace FIAP_Cloud_Games.Domain.Exceptions
{

    public class DomainException : Exception

    {

        /// Código identificador reutilizável (ex: USER_NAME_REQUIRED).
        public string? Code { get; }

        /// Erros detalhados por campo (campo -> mensagens).

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
}
