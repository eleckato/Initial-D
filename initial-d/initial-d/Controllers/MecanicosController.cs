using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.Common;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("mecanicos")]
    public class MecanicosController : Controller
    {
        // GET: Mecanicos
        [HttpGet]
        [Route]
        public ActionResult MechList()
        {
            SetNavbar();
            return View();
        }


        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Mecanicos", "MechList", "Listado de Mecánicos"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}