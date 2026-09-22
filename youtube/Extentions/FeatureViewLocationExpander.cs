using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace youtube.Extentions
{
    /// <summary>
    /// Expands Razor view search locations to support Feature Folders architecture
    /// (/Features/{FeatureName}/{ViewName}.cshtml) alongside standard MVC views.
    /// </summary>
    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            // {0} - Action / View Name
            // {1} - Controller / Feature Name
            // {2} - Area Name (if present)
            return new[]
            {
                "/Features/{1}/{0}.cshtml",
                "/Features/{1}/{0}/{0}.cshtml",
                "/Features/{1}/Views/{0}.cshtml",
                "/Features/{1}/{0}.razor",
                "/Features/Shared/{0}.cshtml",
                "/Views/{1}/{0}.cshtml",
                "/Views/Shared/{0}.cshtml"
            };
        }
    }
}
