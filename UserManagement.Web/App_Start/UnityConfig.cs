using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using UserManagement.Web.Repositories;
using UserManagement.Web.Services;

namespace UserManagement.Web
{
    public static class UnityConfig
    {
        public static void Register()
        {
            UnityContainer container = new UnityContainer();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IAuthService, AuthService>();
            container.RegisterType<IUserService, UserService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}

