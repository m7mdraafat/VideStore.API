using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VideStore.Application.ErrorHandling;

namespace VideStore.Api.Filters
{
    public class ResultFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // No operation before action execution
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is Result result)
            {
                context.Result = HandleResult(result);
            }
            else if (context.Result is ObjectResult genericResult && IsGenericResult(genericResult.Value))
            {
                var resultValue = genericResult.Value;
                var isSuccessProperty = resultValue?.GetType().GetProperty("IsSuccess")?.GetValue(resultValue);

                if (isSuccessProperty is true)
                {
                    context.Result = new OkObjectResult(resultValue);
                }
                else
                {
                    var errorProperty = resultValue?.GetType().GetProperty("Error")?.GetValue(resultValue);
                    context.Result = HandleFailureResult(errorProperty as Error);
                }
            }
        }

        private IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return result.SuccessMessage is not null
                    ? new OkObjectResult(result.SuccessMessage)
                    : new OkResult();
            }
            return HandleFailureResult(result.Error);
        }

        private IActionResult HandleFailureResult(Error? error)
        {
            var statusCode = error?.StatusCode ?? 500;
            var title = error?.Title ?? "An unknown error occurred";
            return new ObjectResult(new { Error = title }) { StatusCode = statusCode };
        }

        private bool IsGenericResult(object? resultValue)
        {
            if (resultValue == null) return false;

            var type = resultValue.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>);
        }
    }
}
