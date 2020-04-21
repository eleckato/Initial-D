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
    [RoutePrefix("usuarios-adm")]
    public class UsuariosController : BaseController
    {
        readonly UsuariosRepository UP = new UsuariosRepository();

        private const string addRoute = "agregar";
        private const string detailsRoute = "{userId}";
        private const string updateRoute = "{userId}/actualizar";
        private const string deleteRoute = "{userId}/eliminar";
        private const string deteledList = "eliminados";

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
        /// <para> /usuarios </para>
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
                if (Usuarios == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                Usuarios.ForEach(user =>
                {
                    user = UP.ProcessUser(user, userTypeLst, userStatusLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
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
        /// <para> /usuarios/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult UserDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();

            Usuario usuario;
            List<UserType> userTypeLst;

            try
            {
                usuario = UP.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuario = UP.ProcessUser(usuario, userTypeLst, userStatusLst);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name");

            return View(usuario);
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update an existing User
        /// <para> /usuarios/{id}/actualizar </para>
        /// </summary>
        [HttpGet]
        [Route(updateRoute)]
        public ActionResult UpdateUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();

            Usuario usuario;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuario = UP.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", usuario.user_type_id);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", usuario.status_id);

            return View(usuario);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to update the data of an User
        /// <para> /usuarios/{id}/actualizar </para>
        /// </summary>
        [HttpPost]
        [Route(updateRoute)]
        public ActionResult UpdateUser(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            try
            {
                var res = UP.UpdateUser(newUser);

                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El Usuario fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("UserDetails", new { newUser.appuser_id });
        }


        /* ---------------------------------------------------------------- */
        /* ADD USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to add an User
        /// <para> /usuarios/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(addRoute)]
        public ActionResult AddUser()
        {
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            Usuario userTemplate;

            try
            {
                userTemplate = new Usuario(true);

                if (userTemplate == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name");
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status");

            return View(userTemplate);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to add an User
        /// <para> /usuarios/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(addRoute)]
        public ActionResult AddUser(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            try
            {
                var res = UP.AddUser(newUser);

                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El Usuario fue agregado con éxito";
            SetSuccessMsg(successMsg);

            // TODO Put the actual appuser_id here when connection to API is implemented
            string userId = "U2"; // newUser.appuser_id;

            return RedirectToAction("UserDetails", new { userId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete an User
        /// <para> /usuarios/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();

            try
            {
                var res = UP.DeleteUser(userId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Usuario fue eliminado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("UserList");
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to restore a deleted User
        /// <para> /Usuarios/RestoreUser </para>
        /// </summary>
        [HttpGet]
        public ActionResult RestoreUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();

            try
            {
                var res = UP.RestoreUser(userId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Usuario fue restaurado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedUserList");
        }

        // TODO Search Filters
        /// <summary>
        /// GET | Show a list of all deleted Users
        /// <para> /usuarios/eliminados </para>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedUserList()
        {
            List<Usuario> usuarios;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuarios = UP.GetDeletedUsers()?.ToList();
                if (usuarios == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuarios.ForEach(user =>
                {
                    user = UP.ProcessUser(user, userTypeLst, userStatusLst);
                });
            }
            catch (Exception e)
            {
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            //ViewBag.userName = userName;
            //ViewBag.userEmail = userEmail;
            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name"/*, userTypeId*/);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status"/*, userStatusId*/);

            return View(usuarios);
        }

        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the type of an User
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userTypeId"> Id of the new Type for the User </param>
        public ActionResult ChangeUsertype(string userId, string userTypeId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userTypeId)) return Error_InvalidForm();

            try
            {
                var res = UP.ChangeUserType(userId, userTypeId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El tipo del Usuario fue actualizado con éxito";
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
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userStatusId)) return Error_InvalidForm();

            try
            {
                var res = true; // UP.ChangeUserStatus(userId, userStatusId);

                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
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
        /* HELPERS */
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