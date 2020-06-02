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
    public class CashierController : BaseController
    {
        readonly SalesCaller SaC = new SalesCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();


        public CashierController()
        {
            SetNavbar();
        }

        [HttpGet]
        public ActionResult CashierIndex(string saleId)
        {
            if (string.IsNullOrEmpty(saleId)) return Error_InvalidUrl();

            SaleVM sale;

            try
            {
                sale = SaC.GetSale(saleId);
                if (sale == null) return Error_FailedRequest();

                sale.cashier_id = User.Identity.GetUserId();

                Session[$"cash_sale_{saleId}"] = sale;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            var payMetLst = new List<string>()
            {
                "Efectivo",
                "Tarjeta de Débito",
                "Tarjeta de Crédito",
            };
            var payMethods = new SelectList(payMetLst);
            ViewBag.payMethodList = payMethods;

            return View(sale);
        }

        [HttpPost]
        public ActionResult CashierIndex(SaleVM model)
        {
            if (model == null) return Error_FailedRequest();

            try
            {
                SaleVM sale = (SaleVM)Session[$"cash_sale_{model.sale_id}"];
                if (sale == null) return Error_FailedRequest();

                var apisale = SaC.GetSale(model.sale_id);
                if (apisale == null) return Error_FailedRequest();

                apisale.cashier_id = sale.cashier_id;
                apisale.sale_status_id = "PAG";
                apisale.sale_date = DateTime.Now;
                apisale.payment_method = model.payment_method;
                apisale.updated_at = DateTime.Now;

                var res = SaC.UpdateSale(apisale);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return RedirectToAction("SaleDetails", "Sales", new { saleId = model.sale_id });
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
                //new NavbarItems("Cashier", "CashierIndex", "Venta"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}