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
        public ActionResult UserList(string userName = null, string userEmail = null, string userTypeId = null, string userStatusId = null)
        {
            List<Usuario> Usuarios;

            Usuarios = UP.GetAllUsers().ToList();

            if (Usuarios == null) return FailedRequest();

            ViewBag.userName = userName;
            ViewBag.userEmail = userEmail;
            ViewBag.userTypeId = userTypeId;
            ViewBag.userStatusId = userStatusId;

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
            if (string.IsNullOrEmpty(userId)) return URLInvalida();

            var usuario = UP.GetUser(userId);

            if(usuario == null) return FailedRequest();

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
            if (string.IsNullOrEmpty(userId)) return URLInvalida();

            var usuario = UP.GetUser(userId);

            if (usuario == null) return FailedRequest();

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
            if (newUser == null) return URLInvalida();

            var res = UP.UpdateUser(newUser);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El usuario fue actualizado con Exito";

            string userId = newUser.appuser_id;

            SetNavbar();
            return RedirectToAction("UserDetails", new { userId });
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

            if (userTemplate == null) return FailedRequest();

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
            if (newUser == null) return URLInvalida();

            var res = UP.AddUser(newUser);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El usuario fue agregado con Exito";
            string userId = "U2"; // newUser.appuser_id;

            SetNavbar();
            return RedirectToAction("UserDetails", new { userId = userId });
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
            if (string.IsNullOrEmpty(userId)) return URLInvalida();

            var res = UP.DeleteUser(userId);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El usuario fue eliminado con Exito";

            Debug.WriteLine($"USER {userId} DELETED");

            SetNavbar();
            return RedirectToAction("UserList");
        }


        [HttpPost]
        public ActionResult ChangeUsertype(string userId, string userTypeId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userTypeId)) return FomularioInvalido();

            var res = true; // UP.ChangeUserType(userId, userTypeId);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El tipo del usuario fue actualizado con con Exito";

            Debug.WriteLine($"USER TYPE UPDATED TO {userTypeId}");

            string referer = GetReferer(Request);

            SetNavbar();
            return Redirect(referer);
        }


        [HttpPost]
        public ActionResult ChangeUserStatus(string userId, string userStatusId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userStatusId)) return FomularioInvalido();

            var res = true; // UP.ChangeUserStatus(userId, userStatusId);

            if (!res) return FailedRequest();

            string msg;
            if (userStatusId.Equals("BAN"))
                msg = "El Usuario fue dado de baja con éxito.";
            else
                msg = "El Usuario fue dado de alta con éxito.";

            TempData["SuccessMessage"] = msg;

            Debug.WriteLine($"USER CHANGED TO {userStatusId}");

            string referer = GetReferer(Request);

            SetNavbar();
            return Redirect(referer);
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

        /// <summary>
        /// Get the referer url of a Request
        /// </summary>
        private string GetReferer(HttpRequestBase request)
        {
            try
            {
                return request.UrlReferrer.AbsoluteUri;
            }
            catch
            {
                return Url.Action("Error", "Home");
            }

        }

        private RedirectResult URLInvalida()
        {
            TempData["ErrorMessage"] = Resources.Messages.Error_URLInvalida;
            return Redirect(GetReferer(Request));
        }
        private RedirectResult FomularioInvalido()
        {
            TempData["ErrorMessage"] = Resources.Messages.Error_FormInvalido;
            return Redirect(GetReferer(Request));
        }
        private RedirectResult FailedRequest()
        {
            TempData["ErrorMessage"] = Resources.Messages.Error_SolicitudFallida;
            return Redirect(GetReferer(Request));
        }
    }
}