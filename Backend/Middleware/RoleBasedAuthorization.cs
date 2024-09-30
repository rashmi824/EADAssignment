using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;



public class RoleBasedAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleBasedAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            var isAdmin = await userService.IsAdministrator(userId);
            if (!isAdmin && context.Request.Path.StartsWithSegments("/api/admin"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }

        await _next(context);
    }
}
