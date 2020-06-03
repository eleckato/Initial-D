using initial_d.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    public class ReservaController : BaseController
    {



        private const string deteledList = "eliminadas";
        private const string detailsRoute = "{resId}";
        private const string updateRoute = "{resId}/actualizar";
        private const string deleteRoute = "{resId}/eliminar";
        private const string restoreRoute = "{resId}/restaurar";
        private const string cancelRoute = "{resId}/cancelar";
        private const string restrictRoute = "{resId}/restringir";


        public ReservaController()
        {
            SetNavbar();
        }


        // GET: Reserva
        public ActionResult Index()
        {
            return View();
        }


        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Set and saves the data for the navbar in a ViewBag
        /// </summary>
        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Reserva", "ResList", "Reservas"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}