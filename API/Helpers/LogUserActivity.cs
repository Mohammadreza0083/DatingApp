using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

/// <summary>
/// Action filter that logs user activity by updating the last activity timestamp
/// </summary>
public class LogUserActivity: IAsyncActionFilter
{
    /// <summary>
    /// Executes the action filter to log user activity
    /// </summary>
    /// <param name="context">The action executing context</param>
    /// <param name="next">The action execution delegate</param>
    public async Task OnActionExecutionAsync
        (ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        // Skip logging if user is not authenticated
        if (resultContext.HttpContext.User.Identity?.IsAuthenticated is false)
        {
            return;
        }
        
        var userId = resultContext.HttpContext.User.GetUserId();
        
        var repo = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();

        if (repo is null)
        {
            return;
        }
        
        var user = await repo.UserRepository.GetUserByIdAsync(userId);

        if (user is null)
        {
            return;
        }
        
        // Update user's last activity timestamp
        user.LastActivity = DateTime.UtcNow;

        await repo.Complete();
    }
}