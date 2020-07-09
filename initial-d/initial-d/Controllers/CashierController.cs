using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Models.APIModels;
using initial_d.Models.Viewmodels;
using Microsoft.AspNet.Identity;

namespace initial_d.Controllers
{
    [Authorize(Roles = "CAJ,TES")]
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

            List<Servicio> recurringServList = new List<Servicio>();

            try
            {
                SaleVM sale = (SaleVM)Session[$"cash_sale_{model.sale_id}"];
                if (sale == null) return Error_FailedRequest();

                //? Cambiado para hacer test a RF-4  
                var res = true; //SaC.CashSale(model, sale.cashier_id);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return RedirectToAction("CashSuccess", new { saleId = model.sale_id });
        }

        [HttpGet]
        public ActionResult CashSuccess(string saleId)
        {
            if (string.IsNullOrEmpty(saleId)) return Error_FailedRequest();

            CashSuccessVM model;
            var BC = new BookingCaller();

            try
            {
                model = new CashSuccessVM()
                {
                    recServList = new List<Servicio>(),
                    saleId = saleId,
                };

                SaleVM sale = SaC.GetSale(saleId);//(SaleVM)Session[$"cash_sale_{saleId}"];
                if (sale == null) return Error_FailedRequest();


                // Si hay algún servicio periódico en la compra
                if (sale.saleItems.Any(x => x.serv != null && x.serv.is_recurring))
                {
                    var recItems = sale.saleItems.Where(x => x.serv != null && x.serv.is_recurring).ToList();
                    foreach (var item in recItems)
                    {
                        model.recServList.Add(item.serv);
                    }
                }

                var servIds = model.recServList.Select(x => x.serv_id).ToList();

                var bookList = BC.GetAllBookings("ACT").ToList()
                    .Where(x => servIds.Contains(x.serv_id))
                    .ToList();
                var restList = BC.GetAllBookRest().ToList()
                    .Where(x => servIds.Contains(x.serv_id))
                    .ToList();

                model.bookList = bookList;
                model.restList = restList;


                var templateBookList = new List<BookingVM>();

                foreach (var serv in model.recServList)
                {
                    var book = new BookingVM()
                    {
                        serv_id = serv.serv_id,
                        appuser_id = sale.appuser_id,
                        booking_id = Guid.NewGuid().ToString().Replace("-", ""),
                        status_booking_id = "ACT",

                        start_date_hour = DateTime.Now.AddDays(1),
                        end_date_hour = DateTime.Now.AddDays(1).AddMinutes(serv.estimated_time),

                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        deleted = false,

                        serv = serv,
                        
                    };

                    templateBookList.Add(book);
                }

                model.templateBookList = templateBookList;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(model);
        }


        public ActionResult ScheduleBook(BookingVM book, string saleId)
        {
            try
            {
                var isAvailable = CheckBookAvailability(book);
                if (isAvailable == null)
                {
                    Error_FailedRequest();
                    return RedirectToAction("CashSuccess", new { saleId });
                }
                else if (isAvailable == false)
                {
                    return RedirectToAction("CashSuccess", new { saleId });
                }


                var res = new BookingCaller().AddBooking(book);

                if (string.IsNullOrEmpty(res))
                {
                    Error_FailedRequest();
                    return RedirectToAction("CashSuccess", new { saleId });
                }

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            SetSuccessMsg("La hora fue agendada con éxito");
            return RedirectToAction("CashSuccess", new { saleId });
        }

        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Revisa la disponibilidad para una reserva, comparando si hay conflicto con el horario de la tienda, otras reservas o restricciones de horario
        /// </summary>
        /// <param name="book"> Reserva a revisar </param>
        [NonAction]
        public bool? CheckBookAvailability(BookingVM book)
        {
            if (book == null || book.start_date_hour == null || book.end_date_hour == null) return null;

            Debug.WriteLine($" ---------------- CHECKING BOOKING AVAILABILITY ---------------- ");

            var BC = new BookingCaller();

            try
            {
                DateTime start = book.start_date_hour ?? default;
                DateTime end = book.end_date_hour ?? default;
                if (start == default || end == default) return null;

                var servId = book.serv_id;

                var bookDate = start.Date;

                var startTime = start.TimeOfDay;
                var endTime = end.TimeOfDay;

                Debug.WriteLine("----------------------------------------------------------------");
                Debug.WriteLine($"SERV : {book.servName}");
                Debug.WriteLine($"CHEDULE : {book.schedule}");
                Debug.WriteLine("----------------------------------------------------------------");

                //!? GET STORE SCHEDULE
                Debug.WriteLine($" ---------------- CHECKING STORE SCHEDULE ---------------- ");
                var storeSche = BC.GetStoreSche();
                if (storeSche == null) return null;

                // Extraer la hora de apertura y cierre de la tienda
                var scheSt = storeSche.start_hour.TimeOfDay;
                var scheEn = storeSche.end_hour.TimeOfDay;
                // Extraer el horario de almuerzo de la tienda
                var scheLunchSt = storeSche.start_lunch_hour.TimeOfDay;
                var scheLunchEn = storeSche.end_lunch_hour.TimeOfDay;

                Debug.WriteLine($"SCHEDULE : {scheSt} - {scheLunchSt} / {scheLunchEn} - {scheEn}");

                // Check por la apertura y cierre de la tienda
                bool checkMorning = startTime >= scheSt && endTime > scheSt;
                bool checkEvening = startTime < scheEn && endTime <= scheEn;
                // Check la hora de almuerzo como una restricción
                bool checkLunch = CheckTimeConflict(startTime, endTime, scheLunchSt, scheLunchEn);

                // Si cualquiera fallo, conflicto
                if (!checkMorning || !checkEvening || !checkLunch)
                {
                    Debug.WriteLine("> CONFLICT FOUND;");
                    SetErrorMsg("El horario seleccionado está fuera el horario de la tienda");
                    return false;
                }


                Debug.WriteLine($" ---------------- CHECKING RESTRICTIONS ---------------- ");
                //!? GET RESTRICTIONS
                var restList = BC.GetAllBookRest(servId).ToList();
                if (restList == null) return null;

                // Saca solo las reservas que son para el mismo día
                restList = restList.Where(x =>
                    x.start_date_hour?.Date == bookDate)
                    .ToList();

                foreach (var rest in restList)
                {
                    DateTime chStart = rest.start_date_hour ?? default;
                    DateTime chEnd = rest.end_date_hour ?? default;
                    if (chStart == default || chEnd == default) return null;

                    Debug.WriteLine($"TIME : {chStart.Hour} - {chEnd.Hour}");

                    var check = CheckTimeConflict(start, end, chStart, chEnd);

                    if (!check)
                    {
                        Debug.WriteLine("> CONFLICT FOUND;");
                        SetErrorMsg("El horario seleccionado no está disponible");
                        return false;
                    }
                }



                Debug.WriteLine($" ---------------- CHECKING OTHER BOOKINGS ---------------- ");
                //!? GET OTHERS BOOKING FROM THIS SERV
                var otherBookList = BC.GetAllBookings(status_booking_id: "ACT", serv_id: servId);
                if (otherBookList == null) return null;

                // Saca solo las reservas que son para el mismo día
                // Recordar sacar a book de la lista
                otherBookList = otherBookList.Where(x =>
                    x.start_date_hour?.Date == bookDate &&
                    !x.booking_id.Equals(book.booking_id))
                    .ToList();

                // Revisar si hay conflicto con otras reservas
                foreach (var bk in otherBookList)
                {
                    DateTime chStart = bk.start_date_hour ?? default;
                    DateTime chEnd = bk.end_date_hour ?? default;
                    if (chStart == default || chEnd == default) return null;

                    Debug.WriteLine($"TIME : {chStart.Hour} - {chEnd.Hour}");

                    var check = CheckTimeConflict(start, end, chStart, chEnd);

                    if (!check)
                    {
                        Debug.WriteLine("> CONFLICT FOUND;");
                        SetErrorMsg("Ya hay otra reserva agendada a esa hora");
                        return false;
                    }
                }

                // Si no hay conflicto, return true
                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        public enum bookingResult
        {
            store_error,
            rest_error,
            book_error,
            success,
            error
        }

        [NonAction]
        private bool CheckTimeConflict(TimeSpan start, TimeSpan end, TimeSpan chStart, TimeSpan chFinish)
        {
            int st_st = TimeSpan.Compare(start, chStart);
            int st_en = TimeSpan.Compare(start, chFinish);
            int en_st = TimeSpan.Compare(end, chFinish);
            int en_en = TimeSpan.Compare(end, chStart);

            var lst = new List<int>() { st_st, st_en, en_st, en_en };

            var checkMinus = lst.All(x => x <= 0);
            var checkPlus = lst.All(x => x >= 0);

            if (checkMinus || checkPlus) return true;
            else return false;
        }
        [NonAction]
        private bool CheckTimeConflict(DateTime start, DateTime end, DateTime chStart, DateTime chFinish)
        {
            int st_st = DateTime.Compare(start, chStart);
            int st_en = DateTime.Compare(start, chFinish);
            int en_st = DateTime.Compare(end, chFinish);
            int en_en = DateTime.Compare(end, chStart);

            var lst = new List<int>() { st_st, st_en, en_st, en_en };

            var checkMinus = lst.All(x => x <= 0);
            var checkPlus = lst.All(x => x >= 0);

            if (checkMinus || checkPlus) return true;
            else return false;
        }



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