﻿using initial_d.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    public class BaseController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }


        public void SetErrorMsg(string errroMessage)
        {
            TempData["ErrorMessage"] = errroMessage;
        }
        public void SetSuccessMsg(string sucessMessage)
        {
            TempData["SuccessMessage"] = sucessMessage;
        }

        /// <summary>
        /// Get the referrer URL of a Request when an error arise to know where to redirect the User wen 
        /// </summary>
        public string GetRefererForError(HttpRequestBase request)
        {
            // Try to get the referrer URL
            string refererUrl = request?.UrlReferrer?.AbsoluteUri;

            // If is null, redirect to Error page
            // Also, if the requested URL is the same as the referrer, then redirect to Error page to avoid an infinite loop
            if (refererUrl == null || request.Url.Equals(refererUrl)) return Url.Action("Error", "Home");

            // If is safe to go back to the referrer URL, redirect there
            return refererUrl;
        }

        /// <summary>
        /// When a URL is malformed, lack arguments or have invalid arguments
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult InvalidUrl()
        {
            SetErrorMsg(Resources.Messages.Error_URLInvalida);
            return Redirect(GetRefererForError(Request));
        }
        /// <summary>
        /// When a received form isn't valid
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult InvalidForm()
        {
            SetErrorMsg(Resources.Messages.Error_FormInvalido);
            return Redirect(GetRefererForError(Request));
        }
        /// <summary>
        /// When an unknown error with the request arises
        /// <para>Set the correct error message in TempData["ErrorMessage"] and return a safe place to redirect the User</para>
        /// </summary>
        public RedirectResult FailedRequest()
        {
            SetErrorMsg(Resources.Messages.Error_SolicitudFallida);
            return Redirect(GetRefererForError(Request));
        }
        /// <summary>
        /// For custom errors. It the error message on TempData["ErrorMessage"] and return a safe place to redirect the User
        /// </summary>
        /// <param name="error"></param>
        public RedirectResult CustomError(string error)
        {
            SetErrorMsg(error);
            return Redirect(GetRefererForError(Request));
        }
    }
}