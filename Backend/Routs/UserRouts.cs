using Microsoft.AspNetCore.Mvc;

namespace Backend.Routes
{
    public static class UserRoutes
    {
        public const string Register = "api/users/register";
        public const string Login = "api/users/login";
        public const string Logout = "api/users/logout";
        public const string ApproveCustomer = "api/users/approve-customer/{customerId}";
        public const string GetUserById = "api/users/{userId}";
        public const string GetAllUsers = "api/users";
        public const string UpdateUser = "api/users/{userId}";
        public const string DeleteUser = "api/users/{userId}";
    }
}
