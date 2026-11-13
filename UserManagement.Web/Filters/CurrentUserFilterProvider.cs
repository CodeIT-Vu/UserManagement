using System.Collections.Generic;
using System.Web.Mvc;

namespace UserManagement.Web.Filters
{
    /// <summary>
    /// Cung cấp filter xác thực khi MVC xử lý request.
    /// </summary>
    public class CurrentUserFilterProvider : IFilterProvider
    {
        /// <summary>
        /// Resolve filter qua Unity thay vì giữ một instance toàn cục.
        /// </summary>
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            CurrentUserFilter filter = DependencyResolver.Current.GetService<CurrentUserFilter>();
            return new[]
            {
                new Filter(filter, FilterScope.Global, null)
            };
        }
    }
}
