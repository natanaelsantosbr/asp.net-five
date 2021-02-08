using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Natanael.Contracts.V1.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.Filters
{
    public class ValidationsFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.ModelState.IsValid)
            {
                var errosInModelState = context.ModelState
                    .Where(a => a.Value.Errors.Count > 0)
                    .ToDictionary(a => a.Key, a => a.Value.Errors.Select(a => a.ErrorMessage)).ToArray();

                var errorResponse = new ErrorResponse();

                foreach (var error in errosInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new ErrorModel
                        {
                            FieldName = error.Key,
                            Message = subError
                        };

                        errorResponse.Erros.Add(errorModel);
                    }
                }

                context.Result = new BadRequestObjectResult(errorResponse);
                return;
            }

            
            await next();




        }
    }
}
