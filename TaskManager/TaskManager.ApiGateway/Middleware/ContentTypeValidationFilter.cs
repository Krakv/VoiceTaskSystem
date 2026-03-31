using Microsoft.AspNetCore.Mvc.Filters;
using TaskManager.Exceptions;

namespace TaskManager.Middleware;

public class ContentTypeValidationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasFromBody = context.ActionDescriptor.Parameters
            .Any(p => p.BindingInfo?.BindingSource == Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body);

        if (hasFromBody &&
            (context.HttpContext.Request.Method == HttpMethods.Post ||
             context.HttpContext.Request.Method == HttpMethods.Put  ||
             context.HttpContext.Request.Method == HttpMethods.Patch))
        {
            var contentType = context.HttpContext.Request.ContentType;
            if (string.IsNullOrEmpty(contentType) ||
                (!contentType.StartsWith("application/json") &&
                 !contentType.StartsWith("text/json")))
            {
                throw new ValidationAppException("UNSUPPORTED_MEDIA_TYPE", "Неподдерживаемые входные данные");
            }
        }
    }
}
