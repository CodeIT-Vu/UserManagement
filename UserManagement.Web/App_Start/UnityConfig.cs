using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using UserManagement.Web.Filters;
using UserManagement.Web.Repositories;
using UserManagement.Web.Services;

namespace UserManagement.Web
{
    /// <summary>
    /// Cấu hình dependency injection bằng Unity.
    /// </summary>
    public static class UnityConfig
    {
        /// <summary>
        /// Ánh xạ interface với repository, service và gắn resolver cho MVC.
        /// </summary>
        public static void Register()
        {
            UnityContainer container = new UnityContainer();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<CurrentUserFilter>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
