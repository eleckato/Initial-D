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



        public void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Mecanicos", "MechList", "Listado de Mecánicos"),
                new NavbarItems("Mecanicos", "AddMech", "Agregar de Mecánico"),
                new NavbarItems("PublicacionesMec", "PubList", "Publicaciones"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }


    }
}