using Camera.Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class CarritoController : Controller
    {
        private CameraShopEntities db = new CameraShopEntities();
        
        public ActionResult Index()
        {
            string NombreUsuario = User.Identity.GetUserName();
            Clientes Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
            if (Cliente == null)
            {
                return RedirectToAction("Create", "Clientes");
            } else
            {
                Pedidos Pedido = Cliente.Pedidos.Where(e => e.Id_Estado == 1).FirstOrDefault();
                return View(Pedido);
            }
        }
        
        public ActionResult Anadir(int id, int Unidades) // Anadimos producto a un detalle de pedido
        {
            string NombreUsuario = User.Identity.GetUserName();
            Clientes Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
            if (Cliente == null)
            {
                return RedirectToAction("Create", "Clientes");
            } else
            {
                Productos Producto = db.Productos.Find(id);
                if (Producto == null)
                {
                    return HttpNotFound();
                } else if (Producto.Catalogo < 1)
                {
                    return RedirectToAction("Productos", "Details", new { id });
                } else
                {
                    Pedidos PedidoSinConfirmar = Cliente.Pedidos.Where(e => e.Id_Estado == 1).FirstOrDefault();
                    if (PedidoSinConfirmar == null)
                    {
                        PedidoSinConfirmar = new Pedidos();
                        PedidoSinConfirmar.Id_Cliente = Cliente.Id;
                        PedidoSinConfirmar.Id_Estado = 1;
                        PedidoSinConfirmar.Fecha_Pedido = DateTime.Now;
                        PedidoSinConfirmar.Fecha_Modificacion = DateTime.Now;
                        db.Pedidos.Add(PedidoSinConfirmar);
                        db.SaveChanges();
                    }
                    Detalles_Pedido NuevoDetalle = new Detalles_Pedido();
                    NuevoDetalle.Id_Pedido = PedidoSinConfirmar.Id;
                    NuevoDetalle.Id_Producto = Producto.Id;
                    NuevoDetalle.IVA = Producto.IVA;
                    NuevoDetalle.Precio_Unitario = Producto.Precio_Unitario;
                    NuevoDetalle.Unidades = Unidades;
                    db.Detalles_Pedido.Add(NuevoDetalle);
                    PedidoSinConfirmar.Fecha_Modificacion = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
        }
        
        public ActionResult Vaciar()
        {
            string NombreUsuario = User.Identity.GetUserName();
            Clientes Cliente = db.Clientes.Where(e => e.Email == NombreUsuario).FirstOrDefault();
            if (Cliente == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            } else
            {
                Pedidos Pedido = Cliente.Pedidos.Where(e => e.Id_Estado == 1).FirstOrDefault();
                if (Pedido == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);    
                } else if (Pedido.Detalles_Pedido.Count() > 0)
                {
                    db.Detalles_Pedido.RemoveRange(Pedido.Detalles_Pedido);
                    Pedido.Fecha_Modificacion = DateTime.Now;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }
    }
}