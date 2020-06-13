using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;
using Microsoft.AspNet.Identity;

namespace initial_d.Controllers
{
    [Authorize(Roles = "ADM,SUP,TES")]
    [RoutePrefix("mecanicos-adm")]
    public class MecanicosController : BaseController
    {
        readonly MecanicosCaller MP = new MecanicosCaller();
        readonly UsuariosCaller UP = new UsuariosCaller();

        private const string addRoute = "agregar";
        private const string detailsRoute = "{mechId}";
        private const string updateRoute = "{mechId}/actualizar";
        private const string deleteRoute = "{mechId}/eliminar";
        private const string deteledList = "eliminados";
        private const string changeStatusRoute = "{mechId}/change-status";

        public MecanicosController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* MECHANICS LIST */
        /* ---------------------------------------------------------------- */
        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Mechanics
        /// <para> /mecanicos-adm </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult MechList(string userName = null, string userRut = null, string userStatusId = null)
        {
            List<Mecanico> mecanicos;
            List<UserStatus> userStatusLst;

            try
            {
                mecanicos = MP.GetAllMech(userName, userRut, userStatusId)?.ToList();
                if (mecanicos == null) return Error_FailedRequest();

                var userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                mecanicos.ForEach(user =>
                {
                    user = (Mecanico)UP.ProcessUser(user, userTypeLst, userStatusLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.userName = userName;
            ViewBag.userRut = userRut;

            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", userStatusId);

            return View(mecanicos);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of all deleted Mechanics
        /// <para> /mecanicos-adm/eliminados </para>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedMechList(string userName = null, string userRut = null, string userStatusId = null)
        {
            List<Mecanico> usuarios;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                usuarios = MP.GetAllMech(userName, userRut, userStatusId, true)?.ToList();
                if (usuarios == null) return Error_FailedRequest();

                // Remove Current User from the list
                string currentUserId = User.Identity.GetUserId();
                var cUser = usuarios.SingleOrDefault(x => x.appuser_id.Equals(currentUserId));
                if (cUser != null) usuarios.Remove(cUser);

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                usuarios.ForEach(user =>
                {
                    user = (Mecanico)UP.ProcessUser(user, userTypeLst, userStatusLst);
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
        /* MECHANIC DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of an User
        /// <para> /mecanicos-adm/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult MechDetails(string mechId)
        {
            if (string.IsNullOrEmpty(mechId)) return Error_InvalidUrl();

            Mecanico mecanico;
            List<UserType> userTypeLst;

            try
            {
                mecanico = MP.GetMech(mechId);
                if (mecanico == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();

                mecanico = (Mecanico)UP.ProcessUser(mecanico, userTypeLst, userStatusLst);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name");

            return View(mecanico);
        }

        /* ---------------------------------------------------------------- */
        /* UPDATE MECHANIC */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update an existing Mechanic
        /// <para> /mecanicos-adm/{id}/actualizar </para>
        /// </summary>
        [HttpGet]
        [Route(updateRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult UpdateMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId)) return Error_InvalidUrl();

            Mecanico mecanico;
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            try
            {
                mecanico = MP.GetMech(mechId);
                if (mecanico == null) return Error_FailedRequest();

                userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("UpdateMech");
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", mecanico.user_type_id);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", mecanico.status_id);

            return View(mecanico);
        }

        /// <summary>
        /// POST  |  API call to update the data of a Mechanic
        /// <para> /mecanicos-adm/{id}/actualizar </para>
        /// </summary>
        [HttpPost]
        [Route(updateRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult UpdateMech(Mecanico newMech)
        {
            if (newMech == null) return Error_InvalidUrl();

            try
            {
                var res = MP.UpdateMech(newMech);

                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("UpdateMech");
            }

            string successMsg = "El Mecánico fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("MechDetails", new { newMech.appuser_id });
        }


        /* ---------------------------------------------------------------- */
        /* ADD MECHANIC */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to add a Mechanic
        /// <para> /mecanicos-adm/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(addRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult AddMech()
        {
            List<UserStatus> userStatusLst;

            Mecanico mechTemplate;

            try
            {
                mechTemplate = new Mecanico(true);
                if (mechTemplate == null) return Error_FailedRequest();

                userStatusLst = UP.GetAllStatus().ToList();
                if (userStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status");

            return View(mechTemplate);
        }

        /// <summary>
        /// POST  |  API call to add a Mechanic
        /// <para> /mecanicos-adm/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(addRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult AddMech(Mecanico newMech)
        {
            if (newMech == null) return Error_InvalidUrl();

            string mechId;

            try
            {
                mechId = MP.AddMech(newMech);

                if (mechId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El Mecánico fue agregado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("MechDetails", new { mechId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE MECHANIC */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Mechanic
        /// <para> /mecanicos-adm/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult DeleteMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId)) return Error_InvalidUrl();

            try
            {
                var res = MP.DeleteMech(mechId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Mecánico fue eliminado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("MechList");
        }

        /// <summary>
        /// POST  |  API call to restore a deleted Mechanic
        /// <para> /Mecanicos/RestoreUser </para>
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult RestoreMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId)) return Error_InvalidUrl();

            try
            {
                var res = MP.RestoreMech(mechId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Mecánico fue restaurado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedMechList");
        }


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the Status of a Mechanic
        /// </summary>
        /// <param name="mechId"> Id of the Mechanic to update </param>
        /// <param name="userStatusId"> Id of the new Status for the Mechanic </param>
        [HttpPost]
        public ActionResult ChangeMechStatus(string mechId, string userStatusId)
        {
            if (string.IsNullOrEmpty(mechId) || string.IsNullOrEmpty(userStatusId)) return Error_InvalidForm();

            try
            {
                var res = MP.ChangeMechStatus(mechId, userStatusId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string bannedMsg = "El Mecánico fue dado de baja con éxito";
            string unbannedMsg = "El Mecánico fue dado de alta con éxito";
            string genericMsg = "El Mecánico del Usuario fue actualizado con éxito";

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

        public void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>();

            InternalNavbar.Add(new NavbarItems("Mecanicos", "MechList", "Listado de Mecánicos"));
            if (isAdm || isTes) InternalNavbar.Add(new NavbarItems("Mecanicos", "AddMech", "Agregar de Mecánico"));
            InternalNavbar.Add(new NavbarItems("PublicacionesMec", "PubList", "Publicaciones"));

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}