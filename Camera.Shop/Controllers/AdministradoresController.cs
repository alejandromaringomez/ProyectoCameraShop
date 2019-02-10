using Camera.Shop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradoresController : Controller
    {

        private ApplicationDbContext context = new ApplicationDbContext();

        // GET: Administradores
        public ActionResult Index(int? Page)
        {
            const int AdministradoresPorPagina = 10;
            if (Page == null)
            {
                Page = 1;
            }
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            IdentityRole RolAdministrador = roleManager.Roles.Where(a => a.Name == "Administrador").First();
            IQueryable<ApplicationUser> Administradores = context.Users.Where(e => e.Roles.FirstOrDefault().RoleId == RolAdministrador.Id).OrderBy(e => e.UserName);
            return View(Administradores.ToPagedList((int)Page, AdministradoresPorPagina));
        }
        
        // GET: Administradores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Administradores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                ApplicationUser Usuario = new ApplicationUser();
                Usuario.UserName = model.Email;
                Usuario.Email = model.Email;
                string Contrasena = model.Password;
                IdentityResult ResultadoCrearAdministrador = userManager.Create(Usuario, Contrasena);
                if (ResultadoCrearAdministrador.Succeeded)
                {
                    userManager.AddToRole(Usuario.Id, "Administrador");
                    return RedirectToAction("Index");
                } else
                {
                    foreach (string Error in ResultadoCrearAdministrador.Errors)
                    {
                        ModelState.AddModelError("", Error);
                    }
                }
            }
            return View(model);
        }
        
    }
}