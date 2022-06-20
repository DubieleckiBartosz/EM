using System.Threading.Tasks;
using EM.IntegrationTests.Setup;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EM.IntegrationTests.Common
{
    public class FakeUserActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = UserSetup.UserPrincipals();

            await next();
        }
    }
}