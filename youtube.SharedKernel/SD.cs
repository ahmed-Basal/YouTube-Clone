using Microsoft.AspNetCore.Mvc.Rendering;

namespace youtube.SharedKernel
{
    public static class SD
    {
        public const string AdminRole = "Admin";
        public const string ModerateRole = "Moderator";
        public const string UserRole = "User";
        public static readonly List<string> Roles = new List<string> { AdminRole, UserRole, ModerateRole };

        public static string IsActive(this IHtmlHelper html, string controller, string action, string cssClass = "active")
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = routeData.Values["action"]?.ToString();
            var routeController = routeData.Values["controller"]?.ToString();

            var returnActive = controller == routeController && action == routeAction;

            return returnActive ? cssClass : string.Empty;
        }
    }
}
