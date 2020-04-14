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
        readonly UsuariosRepository UP = new UsuariosRepository();

        public UsuariosController()
        {
            var statusLst = UP.GetAllStatus();
            var typeLst = UP.GetAllTypes();

            SetNavbar();
        }

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

            Usuarios = UP.GetAllUsers()?.ToList();

            if (Usuarios == null) return FailedRequest();

            ViewBag.userName = userName;
            ViewBag.userEmail = userEmail;
            ViewBag.userTypeId = userTypeId;
            ViewBag.userStatusId = userStatusId;

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
            if (string.IsNullOrEmpty(userId)) return InvalidUrl();

            Usuario usuario;

            try
            {
                usuario = UP.GetUser(userId);

                if (usuario == null) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

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
            if (string.IsNullOrEmpty(userId)) return InvalidUrl();

            var usuario = UP.GetUser(userId);

            if (usuario == null) return FailedRequest();

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
            if (newUser == null) return InvalidUrl();

            var res = UP.UpdateUser(newUser);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El usuario fue actualizado con Exito";

            string userId = newUser.appuser_id;

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
            if (newUser == null) return InvalidUrl();

            var res = UP.AddUser(newUser);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El usuario fue agregado con Exito";
            string userId = "U2"; // newUser.appuser_id;

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
            if (string.IsNullOrEmpty(userId)) return InvalidUrl();

            var res = UP.DeleteUser(userId);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El usuario fue eliminado con Exito";

            Debug.WriteLine($"USER {userId} DELETED");

            return RedirectToAction("UserList");
        }

        /// <summary>
        /// POST  |  API call to update the type of an User
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userTypeId"> Id of the new Type for the User </param>
        [HttpPost]
        public ActionResult ChangeUsertype(string userId, string userTypeId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userTypeId)) return InvalidForm();

            var res = true; // UP.ChangeUserType(userId, userTypeId);

            if (!res) return FailedRequest();

            TempData["SuccessMessage"] = "El tipo del usuario fue actualizado con con Exito";

            Debug.WriteLine($"USER TYPE UPDATED TO {userTypeId}");

            string referer = GetRefererForError(Request);

            return Redirect(referer);
        }

        /// <summary>
        /// POST  |  API call to update the Status of an User
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userStatusId"> Id of the new Status for the User </param>
        [HttpPost]
        public ActionResult ChangeUserStatus(string userId, string userStatusId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userStatusId)) return InvalidForm();

            var res = true; // UP.ChangeUserStatus(userId, userStatusId);

            if (!res) return FailedRequest();

            string msg;
            if (userStatusId.Equals("BAN"))
                msg = "El Usuario fue dado de baja con éxito.";
            else if ((userStatusId.Equals("ACT")))
                msg = "El Usuario fue dado de alta con éxito.";
            else
                msg = "El Status del Usuario fue actualizado con éxito";

            TempData["SuccessMessage"] = msg;

            Debug.WriteLine($"USER CHANGED TO {userStatusId}");

            string referer = GetRefererForError(Request);

            return Redirect(referer);
        }


        /* ---------------------------------------------------------------- */
        /* COMMON */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Set and saves the dat for the navbar in a ViewBag
        /// </summary>
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
        /// Get the referer url of a Request when an error araise to know where to redirect the User
        /// </summary>
        private string GetRefererForError(HttpRequestBase request)
        {
            // Try to get the referer URL
            string refererUrl = request?.UrlReferrer?.AbsoluteUri;

            // If is null, redirect to Error page
            // Also, if the requested URL is the same as the referer, then redirect to Error page to avoid an infinite loop
            if (refererUrl == null || request.Url.Equals(refererUrl)) return Url.Action("Error", "Home");

            // If is safe to go back to the referer url, redirect there
            return refererUrl;
        }

        /// <summary>
        /// When a URL is malformed, lack arguments or have invalid arguments
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        private RedirectResult InvalidUrl()
        {
            TempData["ErrorMessage"] = Resources.Messages.Error_URLInvalida;
            return Redirect(GetRefererForError(Request));
        }
        /// <summary>
        /// When a recieved form isn't valid
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        private RedirectResult InvalidForm()
        {
            TempData["ErrorMessage"] = Resources.Messages.Error_FormInvalido;
            return Redirect(GetRefererForError(Request));
        }
        /// <summary>
        /// When an unkow error with the request arises
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        private RedirectResult FailedRequest()
        {
            TempData["ErrorMessage"] = Resources.Messages.Error_SolicitudFallida;
            return Redirect(GetRefererForError(Request));
        }
        /// <summary>
        /// For custom errors. It the the error message on TempData["ErrorMessage"] and return a safe place to redirect the User
        /// </summary>
        /// <param name="error"></param>
        private RedirectResult CustomError(string error)
        {
            TempData["ErrorMessage"] = error;
            return Redirect(GetRefererForError(Request));
        }
    }
}