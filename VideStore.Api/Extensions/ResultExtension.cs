using Microsoft.AspNetCore.Mvc;
using VideStore.Domain.ErrorHandling;

namespace VideStore.Api.Extensions
{
    public static class ResultExtensions
    {
        public static ActionResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot convert a success result to a problem.");
            }

            var problemDetails = new ProblemDetails
            {
                Status = result.Error?.StatusCode ?? StatusCodes.Status500InternalServerError,
                Title = Error.GetHttpMessage(result.Error.StatusCode),
                Type = null,
                Extensions = new Dictionary<string, object?>
                {
                    { "errors", new[] {result.Error.Description} }
                }
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        public static ActionResult ToSuccess(this Result result, object? value = null, int statusCode = StatusCodes.Status200OK)
        {
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException("Cannot convert a failure result to a success response.");
            }

            var successDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = Error.GetHttpMessage(statusCode),
                Extensions = new Dictionary<string, object?>
                {
                    { "data", value }
                }
            };

            return new ObjectResult(successDetails)
            {
                StatusCode = statusCode
            };
        }


        public static ActionResult<TValue> ToActionResult<TValue>(this Result<TValue> result)
        {
            return result.IsSuccess
                ? (ActionResult<TValue>)new ObjectResult(result.Value)
                {
                    StatusCode = StatusCodes.Status200OK
                }
                : (ActionResult<TValue>)result.ToProblem();
        }
    }
}
