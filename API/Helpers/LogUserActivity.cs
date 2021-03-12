using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Execute and update the LastActive property. 
            // We have a context after the action has been executed
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();

            // Get access to our repo. We can do this using our service locator pattern.
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await uow.UserRepository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.UtcNow;
            await uow.Complete();
        }
    }
}