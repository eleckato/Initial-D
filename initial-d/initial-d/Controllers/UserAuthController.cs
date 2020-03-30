using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    public class UserAuthController : Controller
    {
        // GET: UserAuth

        /// <summary>
        /// GET - Login view
        /// </summary>
        /// <param name="Error"> If there was an error in the operation </param>
        public ActionResult Index(int Error = 0)
        {
            // Check if the user is already loged in, if so, redirect to Home|Index

            // Check for errors and set the Viewbag with a message
            if (Error > 0)
            {
                switch (Error)
                {
                    case 1:
                        ViewBag.ErrorMessage = "";
                        break;
                    case 2:
                        ViewBag.ErrorMessage = "El nombre de usuario o contraseña es incorrecto";
                        break;

                    default:
                        break;
                }
            }

            // Return the Login view
            return View();
        }

        /// <summary>
        /// POST - Try to login the User with the credencials
        /// </summary>
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Check arguments integrity
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return RedirectToAction("Index", "UserAuth", new { Error = 1 });

            // Try to auth the user
            var auth = AuthenticateUser(username, password);

            // Redirect depending on the auth result
            if (auth)
                return RedirectToAction("Index", "Home");
            else
                return RedirectToAction("Index", "UserAuth", new { Error = 2 });

        }

        /// <summary>
        /// GET? - Logoff the User
        /// </summary>
        public ActionResult Logoff()
        {
            // Do the logoff protocol

            return RedirectToAction("Index", "UserAuth");
        }

        /// <summary>
        /// Try to authenticate the User and set the cookies
        /// </summary>
        private bool AuthenticateUser(string username, string password)
        {
            return true;
        }

    }
}