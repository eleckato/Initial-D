using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.Common;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("productos")]
    public class ProductosController : Controller
    {
        // GET: Productos
        [HttpGet]
        [Route]
        public ActionResult ProdList()
        {
            SetNavbar();
            return View();
        }


        // GET: Productos
        [HttpGet]
        [Route("agregar")]
        public ActionResult AddProd()
        {

            SetNavbar();
            return View();
        }


        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Productos", "ProdList", "Listado de Productos"),
                new NavbarItems("Productos", "AddProd", "Agregar Producto"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}