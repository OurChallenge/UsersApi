using FIAP_Cloud_Games.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace FIAP_Cloud_Games.API.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (DomainException ex)
        {
            LogWarning(context, ex);
            var extra = new { code = ex.Code, errors = ex.Errors };
            await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message, extra);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized: {Message} | TraceId: {TraceId}", ex.Message, context.TraceIdentifier);
            await WriteProblem(context, HttpStatusCode.Unauthorized, "Acesso não autorizado.");
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "NotFound: {Message} | TraceId: {TraceId}", ex.Message, context.TraceIdentifier);
            await WriteProblem(context, HttpStatusCode.NotFound, "Recurso não encontrado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error on request {Method} {Path} | TraceId: {TraceId}", context.Request?.Method, context.Request?.Path, context.TraceIdentifier);
            await WriteProblem(context, HttpStatusCode.InternalServerError, "Erro interno inesperado.");
        }
    }

    private void LogWarning(HttpContext ctx, DomainException ex)
    {
        var userId = ctx.User?.FindFirst("sub")?.Value ?? ctx.User?.Identity?.Name;
        _logger.LogWarning(ex, "Domain error: {Message} | Code: {Code} | UserId: {UserId} | Path: {Path} | TraceId: {TraceId}",
            ex.Message, ex.Code, userId, ctx.Request?.Path, ctx.TraceIdentifier);
    }

    private static async Task WriteProblem(HttpContext ctx, HttpStatusCode code, string detail, object? extensions = null)
    {
        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = (int)code;

        var problem = new Dictionary<string, object?>
        {
            ["type"] = "about:blank",
            ["title"] = code.ToString(),
            ["status"] = (int)code,
            ["detail"] = detail,
            ["traceId"] = ctx.TraceIdentifier,
            ["instance"] = ctx.Request?.Path
        };

        if (extensions != null)
            problem["extensions"] = extensions;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(problem, options);
        await ctx.Response.WriteAsync(json);
    }
}
