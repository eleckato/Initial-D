using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("mecanicos-adm")]
    public class MecanicosController : BaseController
    {
        readonly MecanicosRepository MP = new MecanicosRepository();
        readonly UsuariosRepository UP = new UsuariosRepository();

        private const string addRoute = "agregar";
        private const string detailsRoute = "{mechId}";
        private const string updateRoute = "{mechId}/actualizar";
        private const string deleteRoute = "{mechId}/eliminar";
        private const string changeStatusRoute = "{mechId}/change-status";

        public MecanicosController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* MECHANICS LIST */
        /* ---------------------------------------------------------------- */

        // TODO Search Filters
        // TODO Connection with Repository
        /// <summary>
        /// GET | Show a list of Mechanics
        /// <para> /mecanicos </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult MechList()
        {
            List<Mecanico> mecanicos;

            try
            {
                mecanicos = MP.GetAllMech().ToList();
                if (mecanicos == null) return Error_FailedRequest();

                var userTypeLst = UP.GetAllTypes().ToList();
                if (userTypeLst == null) return Error_FailedRequest();

                var userStatusLst = UP.GetAllStatus().ToList();
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

            return View(mecanicos);
        }


        /* ---------------------------------------------------------------- */
        /* MECHANIC DETAILS */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
        /// <summary>
        /// GET  |  Show all the data of an User
        /// <para> /mecanicos/{id} </para>
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

        // TODO Connection with Repository
        /// <summary>
        /// GET  |  Show a form to update an existing Mechanic
        /// <para> /mecanicos/{id}/actualizar </para>
        /// </summary>
        [HttpGet]
        [Route(updateRoute)]
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
                return Error_CustomError(e.Message);
            }

            ViewBag.userTypeLst = new SelectList(userTypeLst, "user_type_id", "name", mecanico.user_type_id);
            ViewBag.userStatusLst = new SelectList(userStatusLst, "status_id", "status", mecanico.status_id);

            return View(mecanico);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to update the data of a Mechanic
        /// <para> /mecanicos/{id}/actualizar </para>
        /// </summary>
        [HttpPost]
        [Route(updateRoute)]
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
                return Error_CustomError(e.Message);
            }

            string successMsg = "El Mecánico fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("MechDetails", new { newMech.appuser_id });
        }


        /* ---------------------------------------------------------------- */
        /* ADD MECHANIC */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
        /// <summary>
        /// GET  |  Show a form to add a Mechanic
        /// <para> /mecanicos/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(addRoute)]
        public ActionResult AddMechanic()
        {
            List<UserType> userTypeLst;
            List<UserStatus> userStatusLst;

            Mecanico mechTemplate;

            try
            {
                mechTemplate = new Mecanico(true);
                if (mechTemplate == null) return Error_FailedRequest();

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

            return View(mechTemplate);
        }

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to add a Mechanic
        /// <para> /mecanicos/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(addRoute)]
        public ActionResult AddMechanic(Mecanico newMech)
        {
            if (newMech == null) return Error_InvalidUrl();

            try
            {
                var res = MP.AddMech(newMech);

                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El Mecánico fue agregado con éxito";
            SetSuccessMsg(successMsg);

            // TODO Put the actual appuser_id here when connection to API is implemented
            string mechId = "U2"; // newUser.appuser_id;

            return RedirectToAction("MechDetails", new { mechId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE MECHANIC */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to delete a Mechanic
        /// <para> /mecanicos/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
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


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
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
                var res = true; // UP.ChangeUserStatus(userId, userStatusId);
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

        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Mecanicos", "MechList", "Listado de Mecánicos"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}