using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("user-admin")]
    public class UsuariosController : Controller
    {

        UsuariosRepository UP = new UsuariosRepository();


        /* ---------------------------------------------------------------- */
        /* USER LIST */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET | Show a list of Users
        /// <para> /user-admin </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult UserList()
        {
            ErrorWriter.InvalidArgumentsError();
            List<Usuario> Usuarios;

            try
            {
                Usuarios = UP.GetAllUsers().ToList();

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                TempData["ErrorMessage"] = "Hubo un error procesando su solicitud";
                return RedirectToAction("Index", "Home");
            }

            SetNavbar();
            return View(Usuarios);
        }


        /* ---------------------------------------------------------------- */
        /* USER DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of an User
        /// <para> /user-admin/{id} </para>
        /// </summary>
        [HttpGet]
        [Route("{userId}")]
        public ActionResult UserDetails(string userId)
        {
            var usuario = UP.GetUser(userId);

            SetNavbar();
            return View(usuario);
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update an existing User
        /// <para> /user-admin/{id}/update </para>
        /// </summary>
        [HttpGet]
        [Route("{userId}/actualizar")]
        public ActionResult UpdateUser(string userId)
        {
            var usuario = UP.GetUser(userId);

            SetNavbar();
            return View(usuario);
        }

        /// <summary>
        /// POST  |  API call to update the data of an User
        /// <para> /user-admin/{id}/update </para>
        /// </summary>
        [HttpPost]
        [Route("{userId}/actualizar")]
        public ActionResult UpdateUser(Usuario newUser)
        {
            var res = UP.UpdateUser(newUser);
            string useriD = newUser.appuser_id;

            SetNavbar();
            return RedirectToAction("UserDetails", useriD);
        }


        /* ---------------------------------------------------------------- */
        /* ADD USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to add an User
        /// <para> /user-admin/agregar </para>
        /// </summary>
        [HttpGet]
        [Route("agregar")]
        public ActionResult AddUser()
        {
            var userTemplate = new Usuario(true);

            SetNavbar();
            return View(userTemplate);
        }

        /// <summary>
        /// POST  |  API call to add an User
        /// <para> /user-admin/agregar </para>
        /// </summary>
        [HttpPost]
        [Route("agregar")]
        public ActionResult AddUser(Usuario newUser)
        {
            var res = UP.AddUser(newUser);
            string userId = newUser.appuser_id;

            SetNavbar();
            return RedirectToAction("UserDetails", userId);
        }


        /* ---------------------------------------------------------------- */
        /* ADD USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete an User
        /// <para> /user-admin/{id}/delete </para>
        /// </summary>
        [HttpGet]
        [Route("{userId}/delete")]
        public ActionResult DeleteUser(string userId)
        {
            //var res = UP.DeleteUser(userId);
            Debug.WriteLine("USER DELETED");

            SetNavbar();
            return RedirectToAction("UserList");
        }


        /* ---------------------------------------------------------------- */
        /* COMMON */
        /* ---------------------------------------------------------------- */

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