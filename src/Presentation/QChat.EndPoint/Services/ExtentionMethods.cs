using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace QChat.EndPoint.Services;

public static class ExtentionMethods
{
    public static JsonResult ToJson(this Result result)
    {
        result.TryGetError(out var error);
        return new JsonResult(new
        { 
            IsFailure=result.IsFailure,
            IsSuccess=result.IsSuccess,
            Error=error
        });
    }
    public static JsonResult ToJson<T>(this Result<T> result)
    {
        result.TryGetValue(out var value, out var error);
        return new JsonResult(new
        { 
            IsFailure=result.IsFailure,
            IsSuccess=result.IsSuccess,
            Error=error,
            Value=value
        });
    }
    public static void ToModelState(this Result result, ModelStateDictionary modelstate)
    {
        foreach(string err in result.Error.Split('.'))
        {
            modelstate.AddModelError(string.Empty, err);
        }
    }
    public static void ToModelState<T>(this Result<T> result, ModelStateDictionary modelstate)
    {
        foreach(string err in result.Error.Split('.'))
        {
            modelstate.AddModelError(string.Empty, err);
        }
    }
}
