using Camera.Shop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Camera.Shop.Startup))]
namespace Camera.Shop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            CrearUsuariosRolesDefecto();
            ConfigureAuth(app);
        }

        public void CrearUsuariosRolesDefecto()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!roleManager.RoleExists("Administrador"))
            {
                // Creamos el rol de administrador
                IdentityRole RolAdministrador = new IdentityRole();
                RolAdministrador.Name = "Administrador";
                IdentityResult ResultadoRolAdministrador = roleManager.Create(RolAdministrador);
                if(ResultadoRolAdministrador == IdentityResult.Success)
                {
                    // Creamos el usuario administrador
                    ApplicationUser Administrador = new ApplicationUser();
                    Administrador.UserName = "admin@camera.shop";
                    Administrador.Email = "admin@camera.shop";
                    string Contrasena = "Admin123";
                    IdentityResult ResultadoAdministrador = userManager.Create(Administrador, Contrasena);
                    if (ResultadoAdministrador.Succeeded)
                    {
                        // Relacionamos el usuario administrador con el rol de administrador
                        IdentityResult ResultadoRelacionAdministrador = userManager.AddToRole(Administrador.Id, "Administrador");
                    }
                }
            }
            if (!roleManager.RoleExists("Usuario"))
            {
                // Creamos el rol para los usuarios
                IdentityRole NuevoRol = new IdentityRole();
                NuevoRol.Name = "Usuario";
                IdentityResult ResultadoRolUsuario = roleManager.Create(NuevoRol);
            }
        }
    }
}
