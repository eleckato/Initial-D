using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.Common;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("servicios")]
    public class ServiciosController : Controller
    {
        // GET: Servicios
        [HttpGet]
        [Route]
        public ActionResult ServList()
        {

            SetNavbar();
            return View();
        }

        // GET: Servicios
        [HttpGet]
        [Route("agregar")]
        public ActionResult AddServ()
        {

            SetNavbar();
            return View();
        }


        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Servicios", "ServList", "Listado de Servicios"),
                new NavbarItems("Servicios", "AddServ", "Agregar Servicio"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}