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
using PagedList;

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Administrador,Usuario")]
    public class PedidosController : Controller
    {
        private CameraShopEntities db = new CameraShopEntities();
        
        // GET: Pedidos
        public ActionResult Index(int? Estado, int? Page)
        {
            const int PedidosPorPagina = 10;
            if (Page == null)
            {
                Page = 1;
            }
            IQueryable<Estados_Pedido> EstadosPedidoConsulta = db.Estados_Pedido;
            IEnumerable<Pedidos> Pedidos;
            if (User.IsInRole("Administrador"))
            {
                Pedidos = db.Pedidos;
            } else
            {
                string NombreUsuario = User.Identity.GetUserName();
                Clientes Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
                if (Cliente == null)
                {
                    return RedirectToAction("Create", "Clientes");
                } else
                {
                    Pedidos = Cliente.Pedidos.Where(e => e.Id_Estado > 1);
                    EstadosPedidoConsulta = EstadosPedidoConsulta.Where(e => e.Id != 1);
                    if(Estado == 1)
                    {
                        Estado = 0; // No permitimos que solicite los pedidos sin confirmar
                    }
                }
            }
            List<Estados_Pedido> EstadosPedidoLista = EstadosPedidoConsulta.ToList();
            EstadosPedidoLista.Insert(0, new Estados_Pedido {
                Id = 0,
                Nombre = "[Todos]",
            });
            SelectList EstadosPedido = new SelectList(EstadosPedidoLista, "Id", "Nombre");
            SelectListItem EstadoSeleccionado = EstadosPedido.First();
            if (Estado != null)
            {
                Estados_Pedido EstadoPedidos = db.Estados_Pedido.Find(Estado);
                if (EstadoPedidos != null)
                {
                    Pedidos = Pedidos.Where(e => e.Id_Estado == Estado);
                    EstadoSeleccionado = EstadosPedido.Where(e => e.Value == EstadoPedidos.Id.ToString()).First();
                }
            }
            EstadoSeleccionado.Selected = true;
            ViewBag.EstadoSeleccionadoFiltro = EstadoSeleccionado;
            ViewBag.EstadosPedidos = EstadosPedido.ToList();
            return View(Pedidos.OrderByDescending(e => e.Fecha_Modificacion).ToPagedList((int)Page, PedidosPorPagina));
        }
        
        public ActionResult Details(int id)
        {
            Pedidos Pedido = db.Pedidos.Find(id);
            if (Pedido == null)
            {
                return HttpNotFound();
            } else
            {
                if (User.IsInRole("Usuario"))
                {
                    string NombreUsuario = User.Identity.GetUserName();
                    if (NombreUsuario != Pedido.Clientes.Email || Pedido.Id_Estado == 1)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }
                }
            }
            return View(Pedido);
        }
        
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            Pedidos Pedido = db.Pedidos.Find(id);
            if (Pedido == null)
            {
                return HttpNotFound();
            } else
            {
                SelectList Estados = new SelectList(db.Estados_Pedido.Where(e => e.Id > 1), "Id", "Nombre", Pedido.Id_Estado);
                ViewBag.IdEstado = Estados.ToList();
                return View(Pedido);
            }
        }
        
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, int IdEstado)
        {
            Estados_Pedido Estado = db.Estados_Pedido.Find(IdEstado);
            if(Estado == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } else
            {
                Pedidos Pedido = db.Pedidos.Find(id);
                if (Pedido == null)
                {
                    return HttpNotFound();
                } else
                {
                    Pedido.Id_Estado = Estado.Id;
                    Pedido.Fecha_Modificacion = DateTime.Now;
                    db.Entry(Pedido).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Pedidos", new { id = Pedido.Id });
                }
            }
        }

        [Authorize(Roles = "Usuario")]
        public ActionResult Confirmar()
        {
            string NombreUsuario = User.Identity.GetUserName();
            Clientes Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
            if (Cliente == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            } else
            {
                Pedidos Pedido = Cliente.Pedidos.Where(e => e.Id_Estado == 1).FirstOrDefault();
                if (Pedido == null || Pedido.Detalles_Pedido.Count() < 1)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                } else
                {
                    foreach (Detalles_Pedido Detalle in Pedido.Detalles_Pedido)
                    {
                        int sobranteCatalogo = Detalle.Productos.Catalogo - Detalle.Unidades;
                        if (sobranteCatalogo < 0)
                        {
                            sobranteCatalogo = 0;
                        }
                        Detalle.Productos.Catalogo = sobranteCatalogo;
                    }
                    Pedido.Id_Estado = 2;
                    Pedido.Fecha_Modificacion = DateTime.Now;
                    db.Entry(Pedido).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Pedidos", new { id = Pedido.Id });
                }
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
