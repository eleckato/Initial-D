using initial_d.APICallers;
using initial_d.Common;
using initial_d.Common.Extensions;
using initial_d.Models.APIModels;
using initial_d.Models.Viewmodels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("point-of-sale")]
    [Authorize(Roles = "VEN,TES")]
    public class PointOfSaleController : BaseController
    {

        readonly ServiciosCaller SC = new ServiciosCaller();
        readonly ProductosCaller PC = new ProductosCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();
        readonly SalesCaller SaC = new SalesCaller();

        public PointOfSaleController()
        {
            SetNavbar();
        }


        [HttpGet]
        //[Authorize(Roles ="VEN")]
        public ActionResult Index()
        {
            PointOfSaleVM model = new PointOfSaleVM();

            try
            {
                var prodList = PC.GetAllProd(string.Empty, string.Empty, "ACT");
                if (prodList == null) return Error_FailedRequest();

                var servList = SC.GetAllServ(string.Empty, "ACT");
                if (servList == null) return Error_FailedRequest();

                var userList = UC.GetAllUsers(string.Empty, string.Empty, "CLI", "ACT");
                if (userList == null) return Error_FailedRequest();

                model.prodList = prodList.ToList();
                model.servList = servList.ToList();
                model.userList = userList.ToList();

                model.venId = User.Identity.GetUserId();

                string saleId = model.id;
                Session[$"sale_{saleId}"] = model;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(model);
        }

        [HttpPost]
        //[Authorize(Roles ="VEN")]
        public ActionResult Index(PointOfSaleVM model)
        {
            if (model == null) return Error_FailedRequest();

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{model.id}"] ?? null;
            if (sale == null) Error_FailedRequest();

            var saleId = sale.id;

            try
            {
                var codeStr = DateTime.Now.ToString("ddMMyyyyhhmmssf");
                long code = long.Parse(codeStr);

                var apiSale = new Sale()
                {
                    sale_id = sale.id,
                    seller_id = sale.venId,
                    appuser_id = sale.user?.appuser_id,
                    sale_status_id = "PEN",
                    code = code,
                    date_order = DateTime.Now,

                    total = sale.total,
                    subtotal = sale.total,
                    payment_method = "",
                    sale_date = null,
                    cashier_id = null,
                    
                    deleted = false,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };

                var apiSaleItemList = new List<SaleItem>();

                sale.soldItems.ForEach(x =>
                {
                    var newProv = new SaleItem()
                    {
                        provision_id = Guid.NewGuid().ToString().Replace("-", ""),
                        product_id = (x.prod != null) ? x.prod.product_id : "",
                        serv_id = (x.serv != null) ? x.serv.serv_id : "",
                        sale_id = sale.id,

                        quantity = x.amount,
                        total = x.total,
                        unit_price = x.price,

                        deleted = false,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };

                    apiSaleItemList.Add(newProv);
                });


                var saleRes = SaC.AddSale(apiSale);
                if (saleRes == null) return Error_FailedRequest();

                var saleItemRes = SaC.AddSaleItems(apiSaleItemList);
                if (saleItemRes == null) return Error_FailedRequest(); 

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            SetSuccessMsg("Pedido generado exitosamente, guardado como Venta Pendiente.");
            return RedirectToAction("SaleDetails", "Sales", new { saleId });
        }


        /* ---------------------------------------------------------------- */
        /* Partial View HTML Getter */
        /* ---------------------------------------------------------------- */

        public string GetAllProdList(string saleId, string name = "")
        {
            if (string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }


            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                var prodList = PC.GetAllProd(string.Empty, name, "ACT").ToList();

                var saleProds = sale.soldItems.Where(x => x.prod != null).ToList();
                var saleProdsIds = saleProds.Select(x => x.id).ToList();

                prodList.RemoveAll(x => saleProdsIds.Contains(x.product_id));
                

                sale.prodList = prodList;

                html = PartialView("Partial/_prodList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;


        }

        public string GetAllServList(string saleId, string name = "")
        {
            if (string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                var servList = SC.GetAllServ(name, "ACT").ToList();

                var saleServs = sale.soldItems.Where(x => x.serv != null).ToList();
                var saleServsIds = saleServs.Select(x => x.id).ToList();

                servList.RemoveAll(x => saleServsIds.Contains(x.serv_id));

                sale.servList = servList;

                html = PartialView("Partial/_servList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;


        }

        public string GetAllUserList(string saleId, string name = "", string rut = "")
        {
            if (string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                var userList = UC.GetAllUsers(name, rut, "CLI", "ACT").ToList();

                var saleUser = sale.user;

                if (saleUser != null) userList.RemoveAll(x => x.appuser_id.Equals(saleUser.appuser_id));

                sale.userList = userList;

                html = PartialView("Partial/_userList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }


        /* ---------------------------------------------------------------- */
        /* Updates to the Sale Session */
        /* ---------------------------------------------------------------- */
        public string AddProd(string prodId, int? quantity, string saleId)
        {
            if (string.IsNullOrEmpty(prodId) || quantity == null || string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            Producto newProd;
            PoSSaleItem newPrest;
            string html;

            try
            {
                newProd = PC.GetProd(prodId);
                if (newProd == null)
                {
                    string error = "ERROR: Error al solicitar el producto a la API";
                    ErrorWriter.CustomError(error);
                    return error;
                }

                newPrest = new PoSSaleItem()
                {
                    amount = quantity ?? 0,
                    prod = newProd,
                    serv = null
                };

                sale.soldItems.Add(newPrest);

                html = PartialView("Partial/_itemList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }

        public string AddServ(string servId, string saleId)
        {
            if (string.IsNullOrEmpty(servId) || string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            Servicio newServ;
            PoSSaleItem newPrest;
            string html;

            try
            {
                newServ = SC.GetServ(servId);
                if (newServ == null)
                {
                    string error = "ERROR: Error al solicitar el servicio a la API";
                    ErrorWriter.CustomError(error);
                    return error;
                }

                newPrest = new PoSSaleItem()
                {
                    amount = 1,
                    serv = newServ,
                    prod = null
                };

                sale.soldItems.Add(newPrest);

                html = PartialView("Partial/_itemList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }

        public string AddUser(string userId, string saleId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            Usuario selectUser;
            string html;

            try
            {
                selectUser = UC.GetUser(userId);
                if (selectUser == null)
                {
                    string error = "ERROR: Error al solicitar el usuario a la API";
                    ErrorWriter.CustomError(error);
                    return error;
                }

                sale.user = selectUser;

                html = PartialView("Partial/_itemList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }

        public string ChangeAmount(string itemId, bool? isPlus, string saleId)
        {
            if (string.IsNullOrEmpty(itemId) || isPlus == null || string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }


            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                int c = (isPlus ?? false) ? 1 : -1 ;

                var item = sale.soldItems.SingleOrDefault(x => x.id.Equals(itemId));

                item.amount += c;
                if (item.amount < 1) item.amount = 1;

                html = PartialView("Partial/_itemList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }

        public string DeleteItem(string itemId, string saleId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                var item = sale.soldItems.SingleOrDefault(x => x.id.Equals(itemId));

                sale.soldItems.Remove(item);

                html = PartialView("Partial/_itemList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }

        public string RemoveUser(string saleId)
        {
            if (string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                sale.user = null;

                html = PartialView("Partial/_itemList", sale).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }

        public string GetDetailHtml(string itemId, string saleId)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(saleId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            PointOfSaleVM sale = (PointOfSaleVM)Session[$"sale_{saleId}"] ?? null;
            if (sale == null)
            {
                string error = "ERROR: No se encontró en Session el PointOfSaleVM solicitado";
                ErrorWriter.CustomError(error);
                return error;
            }

            string html;

            try
            {
                var item = sale.soldItems.SingleOrDefault(x => x.id.Equals(itemId));

                html = PartialView("Partial/_itemDetails", item).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
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
                new NavbarItems("PointOfSale", "Index", "Hacer una Venta"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}