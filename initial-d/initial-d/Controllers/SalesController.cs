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
    [RoutePrefix("ventas")]
    public class SalesController : BaseController
    {
        readonly SalesCaller SaC = new SalesCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();

        private const string deteledList = "eliminados";
        private const string detailsRoute = "{saleId}";
        private const string updateRoute = "{saleId}/actualizar";
        private const string deleteRoute = "{saleId}/eliminar";
        private const string restoreRoute = "{saleId}/restaurar";
        private const string cancelRoute = "{saleId}/cancelar";


        public SalesController()
        {
            SetNavbar();
        }


        /* ---------------------------------------------------------------- */
        /* SALES LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Sales
        /// <para> /ventas </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult SaleList(string code, string statusId)
        {
            List<SaleVM> sales;
            List<SaleStatus> salesStatusList;

            try
            {
                sales = SaC.GetAllSales(code, statusId).ToList();
                if (sales == null) return Error_FailedRequest();

                salesStatusList = SaC.GetAllStatus().ToList();
                if (salesStatusList == null) return Error_FailedRequest();

                sales = sales.OrderByDescending(x => x.sale_status_id).ToList();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.code = code;
            ViewBag.salesStatusList = new SelectList(salesStatusList, "sale_status_id", "name", statusId);

            return View(sales);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of all deleted Sales
        /// <para> /ventas/eliminados </para>
        /// </summary>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedSaleList(string code, string statusId)
        {
            List<SaleVM> sales;
            List<SaleStatus> salesStatusList;

            try
            {
                sales = SaC.GetAllSales(code, statusId, true).ToList();
                if (sales == null) return Error_FailedRequest();

                salesStatusList = SaC.GetAllStatus().ToList();
                if (salesStatusList == null) return Error_FailedRequest();

                sales = sales.OrderByDescending(x => x.sale_status_id).ToList();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.code = code;
            ViewBag.salesStatusList = new SelectList(salesStatusList, "sale_status_id", "name", statusId);

            return View(sales);
        }


        /* ---------------------------------------------------------------- */
        /* SERVICES DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Sale
        /// <para> /ventas/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult SaleDetails(string saleId)
        {
            if (string.IsNullOrEmpty(saleId)) return Error_InvalidUrl();

            SaleVM sale;

            try
            {
                sale = SaC.GetSale(saleId);
                if (sale == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(sale);
        }


        /* ---------------------------------------------------------------- */
        /* DELETE SALE */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Sale
        /// <para> /ventas/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeleteSale(string saleId)
        {
            if (string.IsNullOrEmpty(saleId)) return Error_InvalidUrl();

            try
            {
                var canRes = SaC.ChangeSaleStatus(saleId, "CAN");
                if (!canRes) return Error_FailedRequest();

                var res = SaC.DeleteSale(saleId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Venta fue eliminada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("SaleList");
        }

        /// <summary>
        /// POST  |  API call to restore a deleted Sale
        /// <para> /ventas/{id}/restaurar </para>
        /// </summary>
        [HttpGet]
        [Route(restoreRoute)]
        public ActionResult RestoreSale(string saleId)
        {
            if (string.IsNullOrEmpty(saleId)) return Error_InvalidUrl();

            try
            {
                var res = SaC.RestoreSale(saleId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Venta fue restaurada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedSaleList");
        }


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to cancel a Service
        /// <para> /servicios/cancelar </para>
        /// </summary>
        /// <param name="saleId"> Id of the Sale to cancel </param>
        [HttpGet]
        [Route(cancelRoute)]
        public ActionResult CancelSale(string saleId)
        {
            if (string.IsNullOrEmpty(saleId)) return Error_InvalidForm();

            try
            {
                var res = SaC.ChangeSaleStatus(saleId, "CAN");
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "La Venta fue cancelada";
            SetSuccessMsg(successMsg);

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
                new NavbarItems("Sales", "SaleList", "Listado de Servicios"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }

    }
}