using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("servicios")]
    public class ServiciosController : BaseController
    {
        readonly ServiciosCaller SC = new ServiciosCaller();

        private const string addRoute = "agregar";
        private const string deteledList = "eliminados";
        private const string detailsRoute = "{servId}";
        private const string updateRoute = "{servId}/actualizar";
        private const string deleteRoute = "{servId}/eliminar";
        private const string restoreRoute = "{servId}/restaurar";
        private const string changeStatusRoute = "change-status";

        public ServiciosController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* SERVICE LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Service
        /// <para> /servicios </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult ServList(string name, string statusId)
        {
            List<Servicio> serv;
            List<ServStatus> servStatusLst;

            try
            {
                serv = SC.GetAllServ(name, statusId).ToList();
                if (serv == null) return Error_FailedRequest();

                servStatusLst = SC.GetAllStatus().ToList();
                if (servStatusLst == null) return Error_FailedRequest();

                serv.ForEach(pub =>
                {
                    pub = SC.ProcessServ(pub, servStatusLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            // To keep the state of the search filters when the user make a search
            ViewBag.name = name;
            ViewBag.servStatusLst = new SelectList(servStatusLst, "status_id", "status", statusId);

            return View(serv);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of all deleted Service
        /// <para> /servicios/eliminados </para>
        /// </summary>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedServList(string name, string statusId)
        {
            List<Servicio> serv;
            List<ServStatus> servStatusLst;

            try
            {
                serv = SC.GetAllServ(name, statusId, true).ToList();
                if (serv == null) return Error_FailedRequest();

                servStatusLst = SC.GetAllStatus().ToList();
                if (servStatusLst == null) return Error_FailedRequest();

                serv.ForEach(pub =>
                {
                    pub = SC.ProcessServ(pub, servStatusLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            // To keep the state of the search filters when the user make a search
            ViewBag.name = name;
            ViewBag.servStatusLst = new SelectList(servStatusLst, "status_id", "status", statusId);

            return View(serv);
        }


        /* ---------------------------------------------------------------- */
        /* SERVICES DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Service
        /// <para> /servicios/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult ServDetails(string servId)
        {
            if (string.IsNullOrEmpty(servId)) return Error_InvalidUrl();

            Servicio serv;
            List<ServStatus> servStatusLst;

            try
            {
                serv = SC.GetServ(servId);
                if (serv == null) return Error_FailedRequest();

                servStatusLst = SC.GetAllStatus().ToList();
                if (servStatusLst == null) return Error_FailedRequest();

                serv = SC.ProcessServ(serv, servStatusLst);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(serv);
        }


        /* ---------------------------------------------------------------- */
        /* ADD SERVICE */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to add a Service
        /// <para> /servicios/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(addRoute)]
        public ActionResult AddServ()
        {
            Servicio servTemplate;
            List<ServStatus> servStatusList;

            try
            {
                servTemplate = new Servicio(true);
                if (servTemplate == null) return Error_FailedRequest();

                servStatusList = SC.GetAllStatus().ToList();
                if (servStatusList == null) return Error_FailedRequest();

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.servStatusList = new SelectList(servStatusList, "status_id", "status");

            return View(servTemplate);
        }

        // TODO API CALL
        /// <summary>
        /// POST  |  API call to add a Service
        /// <para> /servicios/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(addRoute)]
        public ActionResult AddServ(Servicio newServ)
        {
            if (newServ == null) return Error_InvalidUrl();

            string servId;

            try
            {
                servId = SC.AddServ(newServ);
                if (servId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("AddServ");
            }

            string successMsg = "El Servicio fue agregado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("ServDetails", new { servId });
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE SERVICE */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update an existing Service
        /// <para> /servicios/{id}/actualizar </para>
        /// </summary>
        [HttpGet]
        [Route(updateRoute)]
        public ActionResult UpdateServ(string servId)
        {
            if (string.IsNullOrEmpty(servId)) return Error_InvalidUrl();

            Servicio serv;
            List<ServStatus> servStatusList;

            try
            {
                serv = SC.GetServ(servId);
                if (serv == null) return Error_FailedRequest();

                servStatusList = SC.GetAllStatus().ToList();
                if (servStatusList == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.servStatusList = new SelectList(servStatusList, "status_id", "status", serv.serv_status);

            return View(serv);
        }

        /// <summary>
        /// POST  |  API call to update the data of a Service
        /// <para> /servicios/{id}/actualizar </para>
        /// </summary>
        [HttpPost]
        [Route(updateRoute)]
        public ActionResult UpdateServ(Servicio newServ)
        {
            if (newServ == null) return Error_InvalidUrl();
            string servId = newServ.serv_id;

            try
            {
                var res = SC.UpdateServ(newServ);

                if (!res)
                {
                    Error_FailedRequest();
                    return RedirectToAction("UpdateServ", new { servId });
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("UpdateServ", new { servId });
            }

            string successMsg = "El Servicio fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("ServDetails", new { servId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Service
        /// <para> /servicios/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeleteServ(string servId)
        {
            if (string.IsNullOrEmpty(servId)) return Error_InvalidUrl();

            try
            {
                var res = SC.DeleteServ(servId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Servicio fue eliminado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("ServList");
        }

        /// <summary>
        /// POST  |  API call to restore a deleted Service
        /// <para> /servicios/{id}/restaurar </para>
        /// </summary>
        [HttpGet]
        [Route(restoreRoute)]
        public ActionResult RestoreServ(string servId)
        {
            if (string.IsNullOrEmpty(servId)) return Error_InvalidUrl();

            try
            {
                var res = SC.RestoreServ(servId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Servicio fue restaurado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedServList");
        }


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the Status of a Service
        /// <para> /servicios/change-status </para>
        /// </summary>
        /// <param name="servId"> Id of the Service to update </param>
        /// <param name="servStatusId"> Id of the new Status for the Service </param>
        [HttpPost]
        [Route(changeStatusRoute)]
        public ActionResult ChangeServStatus(string servId, string servStatusId)
        {
            if (string.IsNullOrEmpty(servId) || string.IsNullOrEmpty(servStatusId)) return Error_InvalidForm();

            try
            {
                var res = SC.ChangeServStatus(servId, servStatusId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string deactivateMsg = "El Servicio fue desactivado con éxito";
            string activateMsg = "El Servicio fue activado con éxito";
            string genericMsg = "El Status del Servicio fue actualizado con éxito";

            string msg;
            if (servStatusId.Equals("INA"))
                msg = deactivateMsg;
            else if ((servStatusId.Equals("ACT")))
                msg = activateMsg;
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
                new NavbarItems("Servicios", "ServList", "Listado de Servicios"),
                new NavbarItems("Servicios", "AddServ", "Agregar Servicio"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}