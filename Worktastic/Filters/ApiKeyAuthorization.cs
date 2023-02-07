using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Worktastic.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthorization : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            //Check api auth
            if (context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var key))
            {
                var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                var configApiKey = config.GetValue<string>("ApiKey");
                if (key.Equals(configApiKey) == false)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}
