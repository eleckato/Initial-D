using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("user-admin")]
    public class UsuariosController : BaseController
    {
        readonly UsuariosRepository UP = new UsuariosRepository();

        private const string AddUserRoute = "agregar";
        private const string UserDetailsRoute = "{userId}";
        private const string UpdateUserRoute = "{userId}/actualizar";
        private const string DeleteUserRoute = "{userId}/delete";

        public UsuariosController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* USER LIST */
        /* ---------------------------------------------------------------- */

        // TODO Search Filters
        /// <summary>
        /// GET | Show a list of Users
        /// <para> /user-admin </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult UserList(string userName = null, string userEmail = null, string userTypeId = null, string userStatusId = null)
        {
            List<Usuario> Usuarios;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                Usuarios = UP.GetAllUsers().ToList();
                if (Usuarios == null) return FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return FailedRequest();

                Usuarios.ForEach(user =>
                {
                    user = UP.ProcessUser(user, userTypeLst, userStatusLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.userName = userName;
            ViewBag.userEmail = userEmail;

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", userTypeId);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", userStatusId);

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
        [Route(UserDetailsRoute)]
        public ActionResult UserDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return InvalidUrl();

            Usuario usuario;
            List<UserType> userTypeLst;

            try
            {
                usuario = UP.GetUser(userId);
                if (usuario == null) return FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return FailedRequest();

                var userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return FailedRequest();

                usuario = UP.ProcessUser(usuario, userTypeLst, userStatusLst);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", usuario.user_type_id);

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
        [Route(UpdateUserRoute)]
        public ActionResult UpdateUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return InvalidUrl();

            Usuario usuario;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuario = UP.GetUser(userId);
                if (usuario == null) return FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", usuario.user_type_id);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", usuario.status_id);

            return View(usuario);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to update the data of an User
        /// <para> /user-admin/{id}/update </para>
        /// </summary>
        [HttpPost]
        [Route(UpdateUserRoute)]
        public ActionResult UpdateUser(Usuario newUser)
        {
            if (newUser == null) return InvalidUrl();

            try
            {
                var res = UP.UpdateUser(newUser);

                if (!res) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            string successMsg = "El usuario fue actualizado con Exito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("UserDetails", new { newUser.appuser_id });
        }


        /* ---------------------------------------------------------------- */
        /* ADD USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to add an User
        /// <para> /user-admin/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(AddUserRoute)]
        public ActionResult AddUser()
        {
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            Usuario userTemplate;

            try
            {
                userTemplate = new Usuario(true);

                if (userTemplate == null) return FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }


            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name");
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status");

            return View(userTemplate);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to add an User
        /// <para> /user-admin/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(AddUserRoute)]
        public ActionResult AddUser(Usuario newUser)
        {
            if (newUser == null) return InvalidUrl();

            try
            {
                var res = UP.AddUser(newUser);

                if (!res) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            string successMsg = "El usuario fue agregado con Exito";
            SetSuccessMsg(successMsg);

            // TODO Put the actual appuser_id here when connection to API is implemented
            string userId = "U2"; // newUser.appuser_id;

            return RedirectToAction("UserDetails", new { userId });
        }


        /* ---------------------------------------------------------------- */
        /* ADD USER */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to delete an User
        /// <para> /user-admin/{id}/delete </para>
        /// </summary>
        [HttpGet]
        [Route(DeleteUserRoute)]
        public ActionResult DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return InvalidUrl();

            try
            {
                var res = UP.DeleteUser(userId);

                if (!res) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }


            string successMsg = "El usuario fue eliminado con Exito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("UserList");
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to update the type of an User
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userTypeId"> Id of the new Type for the User </param>
        [HttpPost]
        public ActionResult ChangeUsertype(string userId, string userTypeId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userTypeId)) return InvalidForm();

            try
            {
                var res = true; // UP.ChangeUserType(userId, userTypeId);

                if (!res) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            string successMsg = "El tipo del usuario fue actualizado con con Exito";
            SetSuccessMsg(successMsg);

            string referer = GetRefererForError(Request);
            return Redirect(referer);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to update the Status of an User
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userStatusId"> Id of the new Status for the User </param>
        [HttpPost]
        public ActionResult ChangeUserStatus(string userId, string userStatusId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userStatusId)) return InvalidForm();

            try
            {
                var res = true; // UP.ChangeUserStatus(userId, userStatusId);

                if (!res) return FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return CustomError(e.Message);
            }

            string bannedMsg = "El Usuario fue dado de baja con éxito";
            string unbannedMsg = "El Usuario fue dado de alta con éxito";
            string genericMsg = "El Status del Usuario fue actualizado con éxito";

            string msg;
            if (userStatusId.Equals("BAN"))
                msg = bannedMsg;
            else if ((userStatusId.Equals("ACT")))
                msg = unbannedMsg;
            else
                msg = genericMsg;

            SetSuccessMsg(msg);

            string referer = GetRefererForError(Request);
            return Redirect(referer);
        }


        /* ---------------------------------------------------------------- */
        /* COMMON */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Set and saves the data for the navbar in a ViewBag
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

    }
}