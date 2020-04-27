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
    [RoutePrefix("mecanicos/publicaciones")]
    public class PublicacionesMecController : BaseController
    {
        readonly PublicacionesMecCaller PMC = new PublicacionesMecCaller();
        readonly MecanicosCaller MC = new MecanicosCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();

        private const string detailsRoute = "{pubId}";
        private const string updateRoute = "{pubId}/actualizar";
        private const string deleteRoute = "{pubId}/eliminar";
        private const string changeStatusRoute = "{pubId}/change-status";
        private const string deteledList = "eliminados";

        public PublicacionesMecController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* PUBLICATION LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Publications
        /// <para> /mecanicos/publicaciones </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult PubList(string comuna, string bussName, string pubTitle, string statusId)
        {
            List<PublicacionMec> pubs;
            List<PublicStatus> pubStatusList;

            try
            {
                pubs = PMC.GetAllPub(comuna, statusId, bussName, pubTitle).ToList();
                if (pubs == null) return Error_FailedRequest();

                pubStatusList = PMC.GetAllStatus().ToList();
                if (pubStatusList == null) return Error_FailedRequest();

                List<Mecanico> mechList = MC.GetAllMech(null, null, null).ToList();

                pubs.ForEach(pub =>
                {
                    pub = PMC.ProcessPub(pub, pubStatusList, mechList);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.comuna = comuna;
            ViewBag.statusId = statusId;
            ViewBag.pubTitle = pubTitle;
            ViewBag.pubStatusList = new SelectList(pubStatusList, "public_status_id", "status_name", statusId);

            return View(pubs);
        }


        // TODO Pagination
        /// <summary>
        /// GET | Show a list of all deleted Publications
        /// <para> /mecanicos/publicaciones/eliminados </para>
        /// </summary>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedPubList(string comuna, string bussName, string pubTitle, string statusId)
        {
            List<PublicacionMec> pubs;
            List<PublicStatus> pubStatusList;

            try
            {
                pubs = PMC.GetAllPub(comuna, statusId, bussName, pubTitle, true).ToList();
                if (pubs == null) return Error_FailedRequest();

                pubStatusList = PMC.GetAllStatus().ToList();
                if (pubStatusList == null) return Error_FailedRequest();

                List<Mecanico> mechList = MC.GetAllMech(null, null, null).ToList();

                pubs.ForEach(pub =>
                {
                    pub = PMC.ProcessPub(pub, pubStatusList, mechList);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.comuna = comuna;
            ViewBag.statusId = statusId;
            ViewBag.pubTitle = pubTitle;

            ViewBag.pubStatusList = new SelectList(pubStatusList, "public_status_id", "status_name", statusId);

            return View(pubs);
        }


        /* ---------------------------------------------------------------- */
        /* PUBLICATION DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Publication
        /// <para> /mecanicos/publicaciones/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult PubDetails(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            PublicacionMec pub;

            try
            {
                pub = PMC.GetPub(pubId);
                if (pub == null) return Error_FailedRequest();

                var pubStatusList = PMC.GetAllStatus().ToList();
                if (pubStatusList == null) return Error_FailedRequest();

                List<Mecanico> mechList = MC.GetAllMech(null, null, null).ToList();

                pub = PMC.ProcessPub(pub, pubStatusList, mechList);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pub);
        }


        /* ---------------------------------------------------------------- */
        /* DELETE PUBLICATION */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Publication
        /// <para> /mecanicos/publicaciones/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeletePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            try
            {
                var res = PMC.DeletePub(pubId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Publicación fue eliminada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("PubList");
        }

        /// <summary>
        /// POST  |  API call to restore a deleted Publication
        /// <para> /PublicacionesMec/RestorePub </para>
        /// </summary>
        [HttpGet]
        public ActionResult RestorePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            try
            {
                var res = PMC.RestorePub(pubId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Publicación fue restaurada con éxito";
            SetSuccessMsg(successMsg);

            // TODO DeletedPubList
            return RedirectToAction("DeletedPubList");
        }


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the Status of a Publication
        /// <para> /PublicacionesMec/ChangeUserStatus </para>
        /// </summary>
        /// <param name="pubId"> Id of the Publication to update </param>
        /// <param name="newStatusId"> Id of the new Status for the Publication </param>
        public ActionResult ChangePubState(string pubId, string newStatusId, string msg = null)
        {
            if (string.IsNullOrEmpty(pubId) || string.IsNullOrEmpty(newStatusId)) return Error_InvalidForm();

            try
            {
                var res = PMC.ChangeStatus(pubId, newStatusId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string contextMsg;
            switch (newStatusId)
            {
                case "REJ":
                    contextMsg = "La Publicación ha sido rechazada.";
                    break;
                case "DEB":
                    contextMsg = "La Publicación ha sido Aceptada.";
                    break;
                case "ACT":
                    contextMsg = "La Publicación ha sido Activada con éxito.";
                    break;
                case "INA":
                    contextMsg = "La Publicación ha sido Desactivada con éxito.";
                    break;
                default:
                    contextMsg = "El Status de la Publicación ha sido actualizado con Éxito";
                    break;
            }

            SetSuccessMsg(msg ?? contextMsg);

            string referer = GetRefererForError(Request);
            return Redirect(referer);
        }


        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        public void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Mecanicos", "MechList", "Listado de Mecánicos"),
                new NavbarItems("Mecanicos", "AddMech", "Agregar de Mecánico"),
                new NavbarItems("PublicacionesMec", "PubList", "Publicaciones")
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }


    }
}