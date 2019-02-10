using Camera.Shop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosSinClienteController : Controller
    {

        private ApplicationDbContext context = new ApplicationDbContext();
        private CameraShopEntities db = new CameraShopEntities();

        // GET: Usuarios
        public ActionResult Index(int? Page)
        {
            const int UsuariosPorPagina = 10;
            if (Page == null)
            {
                Page = 1;
            }
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityRole RolUsuario = roleManager.Roles.Where(a => a.Name == "Usuario").First();
            List<string> Clientes = db.Clientes.Select(e => e.Email).ToList();
            IQueryable<ApplicationUser> UsuariosSinCliente = context.Users.Where(e => e.Roles.FirstOrDefault().RoleId == RolUsuario.Id).Where(e => !Clientes.Contains(e.Email)).OrderBy(e => e.UserName);
            return View(UsuariosSinCliente.ToPagedList((int)Page, UsuariosPorPagina));
        }
        
    }
}
