using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Camera.Shop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Usuario,Administrador")]
    public class ClientesController : Controller
    {
        
        private CameraShopEntities db = new CameraShopEntities();

        // GET: Clientes
        [Authorize(Roles = "Administrador")]
        public ActionResult Index(int? Page)
        {
            const int ClientesPorPagina = 10;
            if(Page == null)
            {
                Page = 1;
            }
            IEnumerable<Clientes> Clientes = db.Clientes;
            IEnumerable<ClienteRanking> RankingClientesGasto = Clientes.Select(x => new ClienteRanking { Id =  x.Id, Nombre = x.Nombre, Gastado = x.Pedidos.Where(c => c.Id_Estado != 1).Sum(a => a.Detalles_Pedido.Sum(b => ((b.Precio_Unitario * b.IVA / 100) + b.Precio_Unitario) * b.Unidades)) }).OrderByDescending(z => z.Gastado).Take(4);
            ViewBag.RankingClientesGasto = RankingClientesGasto.ToList();
            return View(Clientes.OrderByDescending(e => e.Id).ToPagedList((int)Page, ClientesPorPagina));
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            Clientes Cliente = null;
            if (User.IsInRole("Administrador"))
            {
                if(id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                } else
                {
                    Cliente = db.Clientes.Find(id);
                    if (Cliente == null)
                    {
                        return HttpNotFound();
                    }
                }
            } else
            {
                string NombreUsuario = User.Identity.GetUserName();
                Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
                if (Cliente == null)
                {
                    return RedirectToAction("Create", "Clientes");
                }
            }
            return View(Cliente);
        }
        
        // GET: Clientes/Create
        [Authorize(Roles = "Usuario")]
        public ActionResult Create()
        {
            string NombreUsuario = User.Identity.GetUserName();
            Clientes Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
            if(Cliente == null)
            {
                return View();
            } else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }
        
        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Usuario")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Email,Fecha_Nacimineto,Telefono,Direccion,Pais")] Clientes Cliente)
        {
            string NombreUsuario = User.Identity.GetUserName();
            Clientes BuscarClienteCorreo = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
            if(BuscarClienteCorreo == null)
            {
                Cliente.Email = NombreUsuario;
                if (ModelState.IsValid)
                {
                    db.Clientes.Add(Cliente);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                } else
                {
                    return View(Cliente);
                }
            } else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            Clientes Cliente = null;
            if (User.IsInRole("Administrador"))
            {
                if(id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                } else
                {
                    Cliente = db.Clientes.Find(id);
                    if (Cliente == null)
                    {
                        return HttpNotFound();
                    }
                }
            } else
            {
                string NombreUsuario = User.Identity.GetUserName();
                Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
                if(Cliente == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            return View(Cliente);
        }
        
        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Email,Fecha_Nacimineto,Telefono,Direccion,Pais")] Clientes Cliente)
        {
            if (User.IsInRole("Usuario"))
            {
                Cliente.Email = User.Identity.GetUserName();
            }
            if (ModelState.IsValid)
            {
                db.Entry(Cliente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Clientes", new { id = Cliente.Id });
            } else
            {
                return View(Cliente);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
