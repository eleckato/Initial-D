using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Common.Extensions;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("reporte-de-ventas")]
    [Authorize(Roles = "ADM,SUP,TES")]
    public class SalesReportController : BaseController
    {
        readonly SalesCaller SaC = new SalesCaller();


        public SalesReportController()
        {
            SetNavbar();
        }


        /* ---------------------------------------------------------------- */
        /* GET SALES REPORT */
        /* ---------------------------------------------------------------- */

        public ActionResult SalesReport(DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            if (dateStart == null || dateEnd == null || dateStart > DateTime.Today || dateStart > dateEnd)
            {
                dateStart = DateTime.Today.AddDays(-1);
                dateEnd = DateTime.Today;
            }
            else if (dateStart == dateEnd)
            {
                dateEnd = dateEnd?.AddHours(23);
            }

            SalesReport report;

            try
            {
                report = SaC.GetSalesReport(dateStart ?? new DateTime(), dateEnd ?? new DateTime());

                if (report == new SalesReport()) SetErrorMsg("");
                if (report == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.dateStart = dateStart;
            ViewBag.dateEnd = dateEnd;

            return View(report);
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
                new NavbarItems(" SalesReport", "SaleReport", "Reporte de Ventas"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }

    }
}