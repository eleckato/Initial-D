﻿using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("perfil")]
    public class UserProfileController : BaseController
    {
        readonly UsuariosCaller UP = new UsuariosCaller();

        private const string ProfileUrl = "";
        private const string UpdateProfileUrl = "actualizar-datos";
        private const string ChangePasswordUrl = "cambiar-contraseña";

        public UserProfileController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* UPDATE USER PROFILE */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET | Show a view with the current logged User personal data
        /// <para> /perfil </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult Profile()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null) return Error_FailedRequest();

            Usuario usuario;

            try
            {
                usuario = UP.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                var userTypeLst = UP.GetAllTypes().ToList();
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

            return View(usuario);
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE USER PROFILE */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update the current logged User
        /// <para> /perfil/actualizar-datos </para>
        /// </summary>
        [HttpGet]
        [Route(UpdateProfileUrl)]
        public ActionResult UpdateProfile()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null) return Error_FailedRequest();

            Usuario usuario;

            try
            {
                usuario = UP.GetUser(userId);
                if (usuario == null) return Error_FailedRequest();

                var userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(usuario);
        }

        /// <summary>
        /// POST  |  API call to update the data of the current logged User
        /// <para> /perfil/actualizar-datos </para>
        /// </summary>
        [HttpPost]
        [Route(UpdateProfileUrl)]
        public ActionResult UpdateProfile(Usuario newUser)
        {
            if (newUser == null) return Error_InvalidUrl();

            try
            {
                Usuario oldUser = UP.GetUser(newUser.appuser_id);
                if (oldUser == null) return Error_FailedRequest();
                
                oldUser.name = newUser.name;
                oldUser.last_names = newUser.last_names;
                oldUser.adress = newUser.adress;
                oldUser.email = newUser.email;
                oldUser.phone = newUser.phone;
                oldUser.birthday = newUser.birthday;

                var res = UP.UpdateUser(oldUser);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El usuario fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("Profile");
        }


        /* ---------------------------------------------------------------- */
        /* CHANGE PASSWORD */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to change the logged User Password
        /// <para> /perfil/cambiar-contraseña </para>
        /// </summary>
        [HttpGet]
        [Route(ChangePasswordUrl)]
        public ActionResult ChangePassword()
        {
            // Get current userId
            var userId = User.Identity.GetUserId();
            if (userId == null) return Error_FailedRequest();

            return View();
        }


        /// <summary>
        /// POST  |  API call to update the Password of the current logged User
        /// <para> /perfil/cambiar-contraseña </para>
        /// </summary>
        [HttpPost]
        [Route(ChangePasswordUrl)]
        public ActionResult ChangePassword(string newPassword, string oldPassword1, string oldPassword2)
        {
            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(oldPassword1) || string.IsNullOrEmpty(oldPassword2)) return Error_InvalidForm(false);

            if (!oldPassword1.Equals(oldPassword2)) return Error_CustomError("Las contraseñas ingresadas no coinciden");

            try
            {
                var res = UP.UpdatePassword(newPassword, oldPassword1);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message, false);
            }

            string successMsg = "Su contraseña fue cambiada";
            SetSuccessMsg(successMsg);

            return RedirectToAction("Profile");
        }



        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("UserProfile", "Profile", "Perfil"),
                new NavbarItems("UserProfile", "UpdateProfile", "Actualizar Datos"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }

    }
}