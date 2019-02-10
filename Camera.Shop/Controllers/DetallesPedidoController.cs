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

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class DetallesPedidoController : Controller
    {
        private CameraShopEntities db = new CameraShopEntities();
        
        public ActionResult SumarUnidad(int id)
        {
            string NombreUsuario = User.Identity.GetUserName();
            Detalles_Pedido Detalle = db.Detalles_Pedido.Find(id);
            if (Detalle == null)
            {
                return HttpNotFound();
            } else if (NombreUsuario != Detalle.Pedidos.Clientes.Email || Detalle.Pedidos.Id_Estado != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            } else
            {
                Detalle.Unidades += 1;
                db.Entry(Detalle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Carrito");
            }
        }

        public ActionResult QuitarUnidad(int id)
        {
            string NombreUsuario = User.Identity.GetUserName();
            Detalles_Pedido Detalle = db.Detalles_Pedido.Find(id);
            if (Detalle == null)
            {
                return HttpNotFound();
            } else if (NombreUsuario != Detalle.Pedidos.Clientes.Email || Detalle.Pedidos.Id_Estado != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            } else if (Detalle.Unidades < 2)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } else
            {
                Detalle.Unidades -= 1;
                db.Entry(Detalle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Carrito");
            }
        }
        
        public ActionResult Delete(int id)
        {
            string NombreUsuario = User.Identity.GetUserName();
            Detalles_Pedido Detalle = db.Detalles_Pedido.Find(id);
            if (Detalle == null)
            {
                return HttpNotFound();
            } else if (NombreUsuario != Detalle.Pedidos.Clientes.Email || Detalle.Pedidos.Id_Estado != 1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            } else
            {
                db.Entry(Detalle).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index", "Carrito");
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
