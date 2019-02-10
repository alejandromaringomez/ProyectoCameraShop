using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Camera.Shop.Models;
using Microsoft.AspNet.Identity;

namespace Camera.Shop.Controllers
{
    public class HomeController : Controller
    {

        private CameraShopEntities db = new CameraShopEntities();

        public ActionResult Index()
        {
            IQueryable<Productos> ProductosEscaparate = db.Productos.Where(e => e.Escaparate == true).OrderByDescending(e => e.Id);
            IQueryable<Productos> RankingProductosVendidos = db.Productos.OrderByDescending(e => e.Detalles_Pedido.Sum(a => a.Unidades)).Take(4);
            ViewBag.ProductosEscaparate = ProductosEscaparate.ToList();
            ViewBag.RankingProductosVendidos = RankingProductosVendidos.ToList();
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
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