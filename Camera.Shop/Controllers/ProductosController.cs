using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Camera.Shop.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Camera.Shop.Controllers
{
    public class ProductosController : Controller
    {
        private CameraShopEntities db = new CameraShopEntities();
        private string RutaImagenesProductos = "/Content/Images/Productos/";

        // GET: Productos
        public ActionResult Index(int? IdCategoria, string Busqueda, int? Page)
        {
            const int ProductosPorPagina = 10;
            if(Page == null)
            {
                Page = 1;
            }
            Categorias CategoriaDefault = new Categorias
            {
                Id = 0,
                Nombre = "Todas",
            };
            List<Categorias> ListaCategorias = db.Categorias.ToList();
            ListaCategorias.Insert(0, CategoriaDefault);
            IEnumerable<Productos> Productos = db.Productos;
            Categorias CategoriaActual;
            Categorias Categoria = db.Categorias.Find(IdCategoria);
            if(Categoria != null)
            {
                CategoriaActual = Categoria;
                Productos = Productos.Where(e => e.Id_Categoria == Categoria.Id);
            } else
            {
                CategoriaActual = CategoriaDefault;
            }
            if(!string.IsNullOrEmpty(Busqueda))
            {
                Productos = Productos.Where(e => e.Nombre.Contains(Busqueda) || e.Descripcion.Contains(Busqueda));
            }
            ViewBag.CategoriaActual = CategoriaActual;
            ViewBag.Categorias = ListaCategorias;
            return View(Productos.OrderByDescending(e => e.Id).ToPagedList((int)Page, ProductosPorPagina));
        }

        // GET: Productos/Details/5
        public ActionResult Details(int id)
        {
            Productos Producto = db.Productos.Find(id);
            if (Producto == null)
            {
                return HttpNotFound();
            } else
            {
                return View(Producto);
            }
        }

        // GET: Productos/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Id_Categoria = new SelectList(db.Categorias, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Id_Categoria,Nombre,Descripcion,Imagen,Catalogo,Precio_Unitario,IVA,Escaparate")] Productos Producto, HttpPostedFileBase Imagen)
        {
            if (ModelState.IsValid)
            {
                db.Productos.Add(Producto);
                db.SaveChanges();
                Producto.Imagen = Producto.Id + Path.GetExtension(Imagen.FileName);
                db.Entry(Producto).State = EntityState.Modified;
                db.SaveChanges();
                Imagen.SaveAs(Server.MapPath("~" + RutaImagenesProductos + Producto.Imagen));
                return RedirectToAction("Index");
            } else
            {
                ViewBag.Id_Categoria = new SelectList(db.Categorias, "Id", "Nombre", Producto.Id_Categoria);
                return View(Producto);
            }
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            Productos Producto = db.Productos.Find(id);
            if (Producto == null)
            {
                return HttpNotFound();
            } else
            {
                ViewBag.Id_Categoria = new SelectList(db.Categorias, "Id", "Nombre", Producto.Id_Categoria);
                return View(Producto);
            }
        }

        // POST: Productos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Id_Categoria,Nombre,Descripcion,Imagen,Catalogo,Precio_Unitario,IVA,Escaparate")] Productos Producto, HttpPostedFileBase Imagen)
        {
            if (Imagen != null)
            {
                Producto.Imagen = Producto.Id + Path.GetExtension(Imagen.FileName);
                Imagen.SaveAs(Server.MapPath("~" + RutaImagenesProductos + Producto.Imagen));
            }
            if (ModelState.IsValid)
            {
                db.Entry(Producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Productos", new { id = Producto.Id });
            } else
            {
                ViewBag.Id_Categoria = new SelectList(db.Categorias, "Id", "Nombre", Producto.Id_Categoria);
                return View(Producto);
            }
        }
        
        // POST: Productos/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            Productos Producto = db.Productos.Find(id);
            if (Producto == null)
            {
                return HttpNotFound();
            } else
            {
                if(Producto.Detalles_Pedido.Count() < 1)
                {
                    DirectoryInfo Directorio = new DirectoryInfo(Server.MapPath("~" + RutaImagenesProductos));
                    FileInfo[] Imagenes = Directorio.GetFiles(Producto.Imagen);
                    if(Imagenes.Count() > 0)
                    {
                        Imagenes[0].Delete();
                    }
                    db.Productos.Remove(Producto);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                } else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public ActionResult EliminarImagen(int id)
        {
            Productos Producto = db.Productos.Find(id);
            if(!string.IsNullOrEmpty(Producto.Imagen))
            {
                DirectoryInfo Directorio = new DirectoryInfo(Server.MapPath("~" + RutaImagenesProductos));
                FileInfo[] Imagenes = Directorio.GetFiles(Producto.Imagen);
                if (Imagenes.Count() == 1)
                {
                    Imagenes[0].Delete();
                    Producto.Imagen = null;
                    db.Entry(Producto).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Edit", new { id });
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
