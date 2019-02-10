using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Camera.Shop.Models;
using PagedList;

namespace Camera.Shop.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CategoriasController : Controller
    {
        private CameraShopEntities db = new CameraShopEntities();

        // GET: Categorias
        public ActionResult Index(int? Page)
        {
            const int CategoriasPorPagina = 10;
            if(Page == null)
            {
                Page = 1;
            }
            IEnumerable<Categorias> Categorias = db.Categorias.OrderByDescending(e => e.Id);
            return View(Categorias.ToPagedList((int)Page, CategoriasPorPagina));
        }

        // GET: Categorias/Details/5
        public ActionResult Details(int id)
        {
            Categorias Categoria = db.Categorias.Find(id);
            if (Categoria == null)
            {
                return HttpNotFound();
            } else
            {
                return View(Categoria);
            }
        }

        // GET: Categorias/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre")] Categorias Categoria)
        {
            if (ModelState.IsValid)
            {
                db.Categorias.Add(Categoria);
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return View(Categoria);
            }
        }

        // GET: Categorias/Edit/5
        public ActionResult Edit(int id)
        {
            Categorias Categoria = db.Categorias.Find(id);
            if (Categoria == null)
            {
                return HttpNotFound();
            } else
            {
                return View(Categoria);
            }
        }

        // POST: Categorias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre")] Categorias Categoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Categoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return View(Categoria);
            }
        }

        // GET: Categorias/Delete/5
        public ActionResult Delete(int id)
        {
            Categorias Categoria = db.Categorias.Find(id);
            if (Categoria == null)
            {
                return HttpNotFound();
            } else if (Categoria.Productos.Count() > 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } else
            {
                db.Categorias.Remove(Categoria);
                db.SaveChanges();
                return RedirectToAction("Index");
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
