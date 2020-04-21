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
    [RoutePrefix("mecanic/publicaciones")]
    public class PublicacionesMecController : BaseController
    {
        readonly PublicacionesMecRepository PP = new PublicacionesMecRepository();
        readonly MecanicosRepository MP = new MecanicosRepository();
        readonly UsuariosRepository UP = new UsuariosRepository();

        ////private const string addRoute = "agregar";
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

        // TODO Search Filters
        // TODO Connection with Repository
        /// <summary>
        /// GET | Show a list of Publications
        /// <para> /mecanicos-adm/publicaciones </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult PubList()
        {
            List<PublicacionMec> pubs;

            try
            {
                pubs = PP.GetAllPub().ToList();
                if (pubs == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pubs);
        }


        /* ---------------------------------------------------------------- */
        /* PUBLICATION DETAILS */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
        /// <summary>
        /// GET  |  Show all the data of a Publication
        /// <para> /mecanicos-adm/publicaciones/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult PubDetails(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            PublicacionMec pub;

            try
            {
                pub = PP.GetPub(pubId);
                if (pub == null) return Error_FailedRequest();
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
        /// <para> /mecanicos-adm/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeletePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            try
            {
                var res = PP.DeletePub(pubId);
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

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to restore a deleted Publication
        /// </summary>
        [HttpGet]
        public ActionResult RestorePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId)) return Error_InvalidUrl();

            try
            {
                var res = PP.RestorePub(pubId);
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

        // TODO Search Filters
        /// <summary>
        /// GET | Show a list of all deleted Publications
        /// <para> /mechanic-admin/publicaciones/eliminados </para>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedPubList()
        {
            List<PublicacionMec> pubs;

            try
            {
                pubs = PP.GetAllDeletedPub().ToList();
                if (pubs == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(pubs);
        }



        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        // TODO Connection with Repository
        /// <summary>
        /// POST  |  API call to update the Status of a Publication
        /// <para> /PublicacionesMec/ChangeUserStatus/{id} </para>
        /// </summary>
        /// <param name="pubId"> Id of the Publication to update </param>
        /// <param name="newStatusId"> Id of the new Status for the Publication </param>
        public ActionResult ChangePubState(string pubId, string newStatusId, string msg = null)
        {
            if (string.IsNullOrEmpty(pubId) || string.IsNullOrEmpty(newStatusId)) return Error_InvalidForm();

            try
            {
                var res = PP.ChangeStatus(pubId, newStatusId);
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