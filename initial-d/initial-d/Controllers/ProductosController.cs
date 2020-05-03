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
    [RoutePrefix("productos")]
    public class ProductosController : BaseController
    {
        readonly ProductosCaller PC = new ProductosCaller();

        private const string addRoute = "agregar";
        private const string deteledList = "eliminados";
        private const string detailsRoute = "{prodId}";
        private const string updateRoute = "{prodId}/actualizar";
        private const string deleteRoute = "{prodId}/eliminar";
        private const string restoreRoute = "{prodId}/restaurar";
        private const string changeStatusRoute = "change-status";

        public ProductosController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- */
        /* PRODUCTS LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Products
        /// <para> /productos </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult ProdList(string brand, string name, string statusId)
        {
            List<Producto> prods;
            List<ProdStatus> prodStatusLst;

            try
            {
                prods = PC.GetAllProd(brand, name, statusId).ToList();
                if (prods == null) return Error_FailedRequest();

                prodStatusLst = PC.GetAllStatus().ToList();
                if (prodStatusLst == null) return Error_FailedRequest();

                var prodUnitLst = PC.GetAllUnits().ToList();
                if (prodUnitLst == null) return Error_FailedRequest();

                prods.ForEach(pub =>
                {
                    pub = PC.ProcessProd(pub, prodStatusLst, prodUnitLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            // To keep the state of the search filters when the user make a search
            ViewBag.brand = brand;
            ViewBag.name = name;
            ViewBag.prodStatusList = new SelectList(prodStatusLst, "status_id", "status", statusId);

            return View(prods);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of all deleted Products
        /// <para> /productos/eliminados </para>
        /// </summary>
        [HttpGet]
        [Route(deteledList)]
        public ActionResult DeletedProdList(string brand, string name, string statusId)
        {
            List<Producto> prods;
            List<ProdStatus> prodStatusLst;

            try
            {
                prods = PC.GetAllProd(brand, name, statusId, true).ToList();
                if (prods == null) return Error_FailedRequest();

                prodStatusLst = PC.GetAllStatus().ToList();
                if (prodStatusLst == null) return Error_FailedRequest();

                var prodUnitLst = PC.GetAllUnits().ToList();
                if (prodUnitLst == null) return Error_FailedRequest();

                prods.ForEach(pub =>
                {
                    pub = PC.ProcessProd(pub, prodStatusLst, prodUnitLst);
                });
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            // To keep the state of the search filters when the user make a search
            ViewBag.brand = brand;
            ViewBag.name = name;
            ViewBag.prodStatusList = new SelectList(prodStatusLst, "status_id", "status", statusId);

            return View(prods);
        }


        /* ---------------------------------------------------------------- */
        /* PRODUCT DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Product
        /// <para> /productos/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult ProdDetails(string prodId)
        {
            if (string.IsNullOrEmpty(prodId)) return Error_InvalidUrl();

            Producto prod;
            List<ProdStatus> prodStatusLst;

            try
            {
                prod = PC.GetProd(prodId);
                if (prod == null) return Error_FailedRequest();

                prodStatusLst = PC.GetAllStatus().ToList();
                if (prodStatusLst == null) return Error_FailedRequest();

                var prodUnitLst = PC.GetAllUnits().ToList();
                if (prodUnitLst == null) return Error_FailedRequest();

                prod = PC.ProcessProd(prod, prodStatusLst, prodUnitLst);
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(prod);
        }


        /* ---------------------------------------------------------------- */
        /* ADD USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to add a Product
        /// <para> /productos/agregar </para>
        /// </summary>
        [HttpGet]
        [Route(addRoute)]
        public ActionResult AddProd()
        {
            Producto prodTemplate;
            List<ProdStatus> prodStatusList;
            List<ProdUnit> prodUnitList;

            try
            {
                prodTemplate = new Producto(true);
                if (prodTemplate == null) return Error_FailedRequest();

                prodStatusList = PC.GetAllStatus().ToList();
                if (prodStatusList == null) return Error_FailedRequest();

                prodUnitList = PC.GetAllUnits().ToList();
                if (prodUnitList == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.prodStatusList = new SelectList(prodStatusList, "status_id", "status");
            ViewBag.ProdUnitList = new SelectList(prodUnitList, "abbreviation", "plural_name");

            return View(prodTemplate);
        }

        /// <summary>
        /// POST  |  API call to add a Product
        /// <para> /productos/agregar </para>
        /// </summary>
        [HttpPost]
        [Route(addRoute)]
        public ActionResult AddProd(Producto newProd)
        {
            if (newProd == null) return Error_InvalidUrl();

            string prodId;

            try
            {
                prodId = PC.AddProd(newProd);
                if (prodId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("AddProd");
            }

            string successMsg = "El Producto fue agregado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("ProdDetails", new { prodId });
        }


        /* ---------------------------------------------------------------- */
        /* UPDATE PRODUCT */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show a form to update an existing Product
        /// <para> /productos/{id}/actualizar </para>
        /// </summary>
        [HttpGet]
        [Route(updateRoute)]
        public ActionResult UpdateProd(string prodId)
        {
            if (string.IsNullOrEmpty(prodId)) return Error_InvalidUrl();

            Producto prod;
            List<ProdStatus> prodStatusList;
            List<ProdUnit> ProdUnitList;

            try
            {
                prod = PC.GetProd(prodId);
                if (prod == null) return Error_FailedRequest();

                prodStatusList = PC.GetAllStatus().ToList();
                if (prodStatusList == null) return Error_FailedRequest();

                ProdUnitList = PC.GetAllUnits().ToList();
                if (ProdUnitList == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            ViewBag.prodStatusList = new SelectList(prodStatusList, "status_id", "status", prod.product_status);
            ViewBag.ProdUnitList = new SelectList(ProdUnitList, "abbreviation", "plural_name", prod.unit_id);

            return View(prod);
        }

        /// <summary>
        /// POST  |  API call to update the data of a Product
        /// <para> /productos/{id}/actualizar </para>
        /// </summary>
        [HttpPost]
        [Route(updateRoute)]
        public ActionResult UpdateProd(Producto newProd)
        {
            if (newProd == null) return Error_InvalidUrl();
            string prodId = newProd.product_id;

            try
            {
                var res = PC.UpdateProd(newProd);

                if (!res)
                {
                    Error_FailedRequest();
                    return RedirectToAction("UpdateProd", new { prodId });
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("UpdateProd", new { prodId });
            }

            string successMsg = "El Producto fue actualizado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("ProdDetails", new { prodId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE USER */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Product
        /// <para> /productos/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeleteProd(string prodId)
        {
            if (string.IsNullOrEmpty(prodId)) return Error_InvalidUrl();

            try
            {
                var res = PC.DeleteProd(prodId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Producto fue eliminado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("ProdList");
        }

        /// <summary>
        /// POST  |  API call to restore a deleted Product
        /// <para> /productos/{id}/restaurar </para>
        /// </summary>
        [HttpGet]
        [Route(restoreRoute)]
        public ActionResult RestoreProd(string prodId)
        {
            if (string.IsNullOrEmpty(prodId)) return Error_InvalidUrl();

            try
            {
                var res = PC.RestoreProd(prodId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "El Producto fue restaurado con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedProdList");
        }


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to update the Status of a Product
        /// <para> /productos/change-status </para>
        /// </summary>
        /// <param name="prodId"> Id of the Product to update </param>
        /// <param name="prodStatusId"> Id of the new Status for the Product </param>
        [HttpPost]
        [Route(changeStatusRoute)]
        public ActionResult ChangeProdStatus(string prodId, string prodStatusId)
        {
            if (string.IsNullOrEmpty(prodId) || string.IsNullOrEmpty(prodStatusId)) return Error_InvalidForm();

            try
            {
                var res = PC.ChangeProdStatus(prodId, prodStatusId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string deactivateMsg = "El Producto fue desactivado con éxito";
            string activateMsg = "El Producto fue activado con éxito";
            string genericMsg = "El Status del Producto fue actualizado con éxito";

            string msg;
            if (prodStatusId.Equals("INA"))
                msg = deactivateMsg;
            else if ((prodStatusId.Equals("ACT")))
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
                new NavbarItems("Productos", "ProdList", "Listado de Productos"),
                new NavbarItems("Productos", "AddProd", "Agregar Producto"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}