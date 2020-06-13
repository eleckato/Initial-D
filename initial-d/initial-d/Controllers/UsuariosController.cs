using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    [Authorize(Roles="ADM,SUP,TES")]
    [RoutePrefix("usuarios-adm")]
    public class UsuariosController : BaseController
    {
        readonly UsuariosCaller UC = new UsuariosCaller();
        public string currentUserId = "";

        private const string addRoute = "agregar";
        private const string detailsRoute = "{userId}";
        private const string updateRoute = "{userId}/actualizar";
        private const string deleteRoute = "{userId}/eliminar";
        private const string deteledList = "eliminados";

        public UsuariosController()
        {
            SetNavbar();
            currentUserId = User?.Identity?.GetUserId();
        }


        /* ---------------------------------------------------------------- */
        /* USER LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Users
        /// <para> /usuarios-adm </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult UserList(string userName = null, string userRut = null, string userTypeId = null, string userStatusId = null)
        {
            List<Usuario> usuarios;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuarios = UC.GetAllUsers(userName, userRut, userTypeId, userStatusId).ToList();
                if (usuarios == null) return Error_FailedRequest();

                // Remove Current User from the list
                string currentUserId = User.Identity.GetUserId();
                var cUser = usuarios.SingleOrDefault(x => x.appuser_id.Equals(currentUserId));
                if (cUser != null) usuarios.Remove(cUser);

                userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UC.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuarios.ForEach(user =>
                {
                    user = UC.ProcessUser(user, userTypeLst, userStatusLst);
                });

                userTypeLst.Remove(userTypeLst.FirstOrDefault(x => x.user_type_id.Equals("TES")));
                if (!User.IsInRole("ADM"))
                {
                    userTypeLst.Remove(userTypeLst.FirstOrDefault(x => x.user_type_id.Equals("ADM")));
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.userName = userName;
            ViewBag.userRut = userRut;

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", userTypeId);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", userStatusId);

            return View(usuarios);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of all deleted Users
        /// <para> /usuarios-adm/eliminados </para>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedUserList(string userName = null, string userRut = null, string userTypeId = null, string userStatusId = null)
        {
            List<Usuario> usuarios;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuarios = UC.GetAllUsers(userName, userRut, userTypeId, userStatusId, true)?.ToList();
                if (usuarios == null) return Error_FailedRequest();

                // Remove Current User from the list
                string currentUserId = User.Identity.GetUserId();
                var cUser = usuarios.SingleOrDefault(x => x.appuser_id.Equals(currentUserId));
                if (cUser != null) usuarios.Remove(cUser);

                userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UC.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuarios.ForEach(user =>
                {
                    user = UC.ProcessUser(user, userTypeLst, userStatusLst);
                });
            }
            catch (Exception e)
            {
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.userName = userName;
            ViewBag.userRut = userRut;
            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name");
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status");

            return View(usuarios);
        }


        /* ---------------------------------------------------------------- */
        /* USER DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of an User
        /// <para> /usuarios-adm/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult UserDetails(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();
            if (userId.Equals(currentUserId)) return Error_FailedRequest();

            Usuario usuario;
            List<UserType> userTypeLst;

            try
            {
                usuario = UC.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UC.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuario = UC.ProcessUser(usuario, userTypeLst, userStatusLst);

                userTypeLst.Remove(userTypeLst.FirstOrDefault(x => x.user_type_id.Equals("TES")));
                if (!User.IsInRole("ADM"))
                {
                    userTypeLst.Remove(userTypeLst.FirstOrDefault(x => x.user_type_id.Equals("ADM")));
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", usuario.user_type_id);

            return View(usuario);
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update an existing User
        /// <para> /usuarios-adm/{id}/actualizar </para>
        /// </summary>
        [HttpGet]
        [Route(updateRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult UpdateUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();
            if (userId.Equals(currentUserId)) return Error_FailedRequest();

            Usuario usuario;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuario = UC.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UC.GetAllStatus().ToList();
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

        /// <summary>
        /// POST  |  API call to update the data of an User
        /// <para> /usuarios-adm/{id}/actualizar </para>
        /// </summary>
        [HttpPost]
        [Route(updateRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult UpdateUser(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();
            if (newUser.appuser_id.Equals(currentUserId)) return Error_FailedRequest();

            try
            {
                var res = UC.UpdateUser(newUser);

                if (!res)
                {
                    Error_FailedRequest();
                    return RedirectToAction("UpdateUser", new { userId = newUser.appuser_id });
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("UpdateUser", new { userId = newUser.appuser_id });
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
        /// <para> /usuarios-adm/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(addRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult AddUser()
        {
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            Usuario userTemplate;

            try
            {
                userTemplate = new Usuario(true);

                if (userTemplate == null) return Error_FailedRequest();

                userTypeLst = UC.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UC.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name");
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", "ACT");

            return View(userTemplate);
        }

        /// <summary>
        /// POST  |  API call to add an User
        /// <para> /usuarios-adm/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(addRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult AddUser(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            string userId;

            try
            {
                userId = UC.RegisterUser(newUser);

                if (userId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("AddUser");
            }

            string successMsg = "El Usuario fue agregado con éxito";
            SetSuccessMsg(successMsg);

            // TODO Put the actual appuser_id here when connection to API is implemented
            //string userId = "U2"; // newUser.appuser_id;

            return RedirectToAction("UserDetails", new { userId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete an User
        /// <para> /usuarios-adm/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();
            if (userId.Equals(currentUserId)) return Error_FailedRequest();

            try
            {
                var res = UC.DeleteUser(userId);
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

        /// <summary>
        /// POST  |  API call to restore a deleted User
        /// <para> /Usuarios/RestoreUser </para>
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult RestoreUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return Error_InvalidUrl();
            if (userId.Equals(currentUserId)) return Error_FailedRequest();

            try
            {
                var res = UC.RestoreUser(userId);
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


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the type of an User
        /// <para> /Usuarios/ChangeUsertype </para>
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userTypeId"> Id of the new Type for the User </param>
        public ActionResult ChangeUsertype(string userId, string userTypeId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userTypeId)) return Error_InvalidForm();
            if (userId.Equals(currentUserId)) return Error_FailedRequest();

            try
            {
                var res = UC.ChangeUserType(userId, userTypeId);
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

        /// <summary>
        /// POST  |  API call to update the Status of an User
        /// <para> /Usuarios/ChangeUserStatus </para>
        /// </summary>
        /// <param name="userId"> Id of the User to update </param>
        /// <param name="userStatusId"> Id of the new Status for the User </param>
        [HttpPost]
        public ActionResult ChangeUserStatus(string userId, string userStatusId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userStatusId)) return Error_InvalidForm();
            if (userId.Equals(currentUserId)) return Error_FailedRequest();

            try
            {
                var res = UC.ChangeUserStatus(userId, userStatusId);

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
            };

            if (isAdm || isTes)
            {
                InternalNavbar.Add(new NavbarItems("Usuarios", "AddUser", "Agregar Usuario"));
            }

            ViewBag.InternalNavbar = InternalNavbar;
        }

    }
}