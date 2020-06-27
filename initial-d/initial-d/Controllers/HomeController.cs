using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController()
        {
            SetNavbar();
        }

        public ActionResult Index()
        {
            List<Producto> lowStockProds;

            try
            {
                lowStockProds = new ProductosCaller().GetLowStockProds();

                ViewBag.lowStockProds = lowStockProds;
            }
            catch (Exception)
            {
                ViewBag.lowStockProds = null;
                return View();
            }

            return View();
        }


        [Route("pagina-no-encontrada")]
        public ActionResult Error()
        {
            ViewBag.InternalNavbar = new List<NavbarItems>();
            return View();
        }

        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Home", "Index", "Inicio"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}