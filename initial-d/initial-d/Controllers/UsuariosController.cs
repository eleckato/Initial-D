using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("user-admin")]
    public class UsuariosController : Controller
    {

        UsuariosRepository UP = new UsuariosRepository();

        // GET: user-admin
        [HttpGet]
        [Route]
        public ActionResult UserList()
        {
            var Usuarios = UP.GetAllUsers();

            SetNavbar();
            return View(Usuarios);
        }

        // GET: user-admin/
        [HttpGet]
        [Route("{userId}")]
        public ActionResult UserDetails(string userId)
        {
            ViewBag.userId = userId;

            SetNavbar();
            return View();
        }

        // GET: user-admin/agregar
        [HttpGet]
        [Route("agregar")]
        public ActionResult AddUser()
        {
            SetNavbar();
            return View();
        }

        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Usuarios", "UserList", "Listado de Usuarios"),
                new NavbarItems("Usuarios", "AddUser", "Agregar Usuario"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}