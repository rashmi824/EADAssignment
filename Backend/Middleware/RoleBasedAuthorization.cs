// RoleBasedAuthorizationMiddleware.cs
// This middleware checks if the user has the appropriate role for accessing certain routes.
// Specifically, it restricts access to admin routes for non-administrator users.

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
        // Retrieve the user ID from the claims
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            // Check if the user is an administrator
            var isAdmin = await userService.IsAdministrator(userId);
            
            // If the user is not an admin and is trying to access admin routes
            if (!isAdmin && context.Request.Path.StartsWithSegments("/api/admin"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden; // Forbidden response
                return; // Short-circuit the request pipeline
            }
        }

        await _next(context); // Call the next middleware in the pipeline
    }
}
