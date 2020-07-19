using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using initial_d.APICallers;
using initial_d.Common;
using initial_d.Common.Extensions;
using initial_d.Models.APIModels;

namespace initial_d.Controllers
{
    [Authorize]
    [RoutePrefix("reservas")]
    [Authorize(Roles = "ADM,SUP,CAJ,VEN,TES")]
    public class BookingController : BaseController
    {
        readonly BookingCaller BC = new BookingCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();
        readonly ServiciosCaller SC = new ServiciosCaller();

        private const string deteledList = "eliminadas";
        private const string detailsRoute = "{bookId}";
        private const string updateRoute = "{bookId}/actualizar";
        private const string deleteRoute = "{bookId}/eliminar";
        private const string restoreRoute = "{bookId}/restaurar";
        private const string cancelRoute = "{bookId}/cancelar";
        private const string restrictRoute = "{bookId}/restringir";


        public BookingController()
        {
            SetNavbar();
        }

        /* ---------------------------------------------------------------- BOOKINGS ---------------------------------------------------------------- */

        /* ---------------------------------------------------------------- */
        /* BOOKING LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Booking
        /// <para> /reservas </para>
        /// </summary>
        [HttpGet]
        [Route]
        public ActionResult BookList(string statusId, string userRut)
        {
            List<BookingVM> bookList;
            List<BookingStatus> bookStatusLst;
            StoreSchedule storeSche;
            string userId = string.Empty;

            if (!string.IsNullOrEmpty(userRut))
            {
                try
                {
                    var userLst = UC.GetAllUsers(string.Empty, userRut, "CLI", "ACT");

                    Usuario user = null;

                    if (userLst.Any())
                    {
                        user = userLst.FirstOrDefault(x => x.rut.Equals(userRut));
                    }

                    userId = user?.appuser_id ?? "null";

                    if (userId.Equals("null")) userId = "null";

                }
                catch { userId = "null"; }
            }

            try
            {
                storeSche = BC.GetStoreSche();
                if (storeSche == null) return Error_FailedRequest();

                bookList = BC.GetAllBookings(statusId, userId).ToList();
                if (bookList == null) return Error_FailedRequest();

                bookList = bookList.OrderBy(x => x.status_booking_id).ToList();


                bookStatusLst = BC.GetAllBookStatus().ToList();
                if (bookStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.userRut = userRut;
            ViewBag.bookStatusLst = new SelectList(bookStatusLst, "status_booking_id", "name", statusId);

            ViewBag.storeSche = storeSche;


            return View(bookList);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Deleted Booking
        /// <para> /reservas/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deteledList)]
        [Authorize(Roles = "ADM,SUP,TES")]
        public ActionResult DeletedBookList(string statusId, string userRut)
        {
            List<BookingVM> bookList;
            List<BookingStatus> bookStatusLst;
            string userId = string.Empty;

            if (!string.IsNullOrEmpty(userRut))
            {
                try
                {
                    var userLst = UC.GetAllUsers(string.Empty, userRut, "CLI", "ACT");

                    Usuario user = null;

                    if (userLst.Any())
                    {
                        user = userLst.FirstOrDefault(x => x.rut.Equals(userRut));
                    }

                    userId = user?.appuser_id ?? "null";

                    if (userId.Equals("null")) userId = "null";

                }
                catch { userId = "null"; }
            }

            try
            {


                bookList = BC.GetAllBookings(statusId, userId, deleted: true).ToList();
                if (bookList == null) return Error_FailedRequest();

                bookStatusLst = BC.GetAllBookStatus().ToList();
                if (bookStatusLst == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.userRut = userRut;
            ViewBag.bookStatusLst = new SelectList(bookStatusLst, "status_booking_id", "name", statusId);

            return View(bookList);
        }


        /* ---------------------------------------------------------------- */
        /* BOOKING DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// GET  |  Show all the data of a Booking
        /// <para> /reservas/{id} </para>
        /// </summary>
        [HttpGet]
        [Route(detailsRoute)]
        public ActionResult BookDetails(string bookId)
        {
            if (string.IsNullOrEmpty(bookId)) return Error_InvalidUrl();

            BookingVM book;

            try
            {
                book = BC.GetBook(bookId);
                if (book == null) return Error_FailedRequest();

                var bookList = BC.GetAllBookings("ACT").ToList()
                    .Where(x => x.serv_id.Equals(book.serv_id) && x.start_date_hour > DateTime.Now)
                    .ToList();
                var restList = BC.GetAllBookRest().ToList()
                    .Where(x => x.serv_id.Equals(book.serv_id) && x.start_date_hour > DateTime.Now)
                    .ToList();

                ViewBag.bookList = bookList;
                ViewBag.restList = restList;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(book);
        }


        /* ---------------------------------------------------------------- */
        /* RESCHEDULE DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to reschedule the data of a Booking
        /// </summary>
        [HttpPost]
        public ActionResult RescheduleBook(BookingVM newBook)
        {
            if (newBook == null) return Error_InvalidUrl();
            string bookId = newBook.booking_id;

            try
            {
                var isAvailable = CheckBookAvailability(newBook);
                if (isAvailable == null)
                { 
                    Error_FailedRequest();
                    return RedirectToAction("BookDetails", new { bookId });
                }
                else if (isAvailable == false)
                {
                    return RedirectToAction("BookDetails", new { bookId });
                }
                
                Booking apiNewBook = newBook;

                var res = BC.UpdateBooking(apiNewBook);

                if (!res)
                {
                    Error_FailedRequest();
                    return RedirectToAction("BookDetails", new { bookId });
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("BookDetails", new { bookId });
            }

            string successMsg = "La Reserva fue reagendada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("BookDetails", new { bookId });
        }


        /* ---------------------------------------------------------------- */
        /* DELETE/RESTORE BOOKING */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Booking
        /// <para> /reservas/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult DeleteBook(string bookid)
        {
            if (string.IsNullOrEmpty(bookid)) return Error_InvalidUrl();

            try
            {
                var book = BC.GetBook(bookid);
                if (book == null) return Error_FailedRequest();

                if (book.status_booking_id.Equals("ACT"))
                {
                    var canRes = BC.ChangeBookStatus(bookid, "CAN");
                    if (!canRes) return Error_FailedRequest();
                }

                var res = BC.DeleteBooking(bookid);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Reserva fue eliminada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("BookList");
        }

        /// <summary>
        /// POST  |  API call to delete a Booking
        /// <para> /reservas/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(restoreRoute)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult RestoreBook(string bookid)
        {
            if (string.IsNullOrEmpty(bookid)) return Error_InvalidUrl();

            try
            {
                var res = BC.RestoreBooking(bookid);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Reserva fue restaurada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedBookList");
        }

        /* ---------------------------------------------------------------- */
        /* CANCEL BOOKING */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to cancel a Booking
        /// <para> /reservas/{bookId}/cancelar </para>
        /// </summary>
        /// <param name="bookId"> Id of the Booking to cancel </param>
        [HttpGet]
        [Route(cancelRoute)]
        public ActionResult CancelBook(string bookId)
        {
            if (string.IsNullOrEmpty(bookId)) return Error_InvalidForm();

            try
            {
                var res = BC.ChangeBookStatus(bookId, "CAN");
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "La Reserva fue cancelada";
            SetSuccessMsg(successMsg);

            string referer = GetRefererForError(Request);
            return Redirect(referer);
        }


        /* ---------------------------------------------------------------- RESTRICTIONS ---------------------------------------------------------------- */
        private const string listRest = "restricciones";
        private const string expiredListRest = "restricciones/expiradas";
        private const string deteledListRest = "restricciones/eliminadas";
        private const string updateRouteRest = "restricciones/{restId}/actualizar";
        private const string deleteRouteRest = "restricciones/{restId}/eliminar";
        private const string restoreRouteRest = "restricciones/{restId}/restaurar";

        /* ---------------------------------------------------------------- */
        /* RESTRICTION LIST */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Restrictions for today onwards
        /// <para> /restricciones </para>
        /// </summary>
        [HttpGet]
        [Route(listRest)]
        public ActionResult RestList(string servId)
        {
            List<BookingRestVM> restList;
            SelectList servList;

            try
            {
                restList = BC.GetAllBookRest(servId).ToList();
                if (restList == null) return Error_FailedRequest();

                restList = restList.Where(x => x.start_date_hour?.Date >= DateTime.Now.Date).ToList();


                var servListApi = SC.GetAllServ(string.Empty, "ACT");
                servList = new SelectList(servListApi, "serv_id", "name");
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.servId = servId;
            ViewBag.servList = servList;

            return View(restList);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Restrictions Expired
        /// <para> /restricciones </para>
        /// </summary>
        [HttpGet]
        [Route(expiredListRest)]
        [Authorize(Roles = "ADM,SUP,TES")]
        public ActionResult ExpiredRestList(string servId)
        {
            List<BookingRestVM> restList;

            try
            {
                restList = BC.GetAllBookRest(servId).ToList();
                if (restList == null) return Error_FailedRequest();

                restList = restList.Where(x => x.start_date_hour?.Date < DateTime.Now.Date).ToList();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.servId = servId;

            return View(restList);
        }

        // TODO Pagination
        /// <summary>
        /// GET | Show a list of Deleted Restrictions
        /// <para> /restricciones/eliminadas </para>
        /// </summary>
        [HttpGet]
        [Route(deteledListRest)]
        [Authorize(Roles = "ADM,SUP,TES")]
        public ActionResult DeletedRestList(string servId)
        {
            List<BookingRestVM> restList;

            try
            {
                restList = BC.GetAllBookRest(servId, true).ToList();
                if (restList == null) return Error_FailedRequest();

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            // To keep the state of the search filters when the user make a search
            ViewBag.servId = servId;

            return View(restList);
        }


        /* ---------------------------------------------------------------- */
        /* ADD RESTRICTION */
        /* ---------------------------------------------------------------- */

        [Authorize(Roles = "ADM,SUP,TES")]
        public ActionResult AddRest(BookingRestVM newRest)
        {
            if (newRest == null) return Error_InvalidUrl();

            string restId;

            try
            {
                BookingRestriction newRestApi = new BookingRestriction();
                newRestApi = PropertyCopier.Copy(newRest, newRestApi);

                restId = BC.AddBookRest(newRestApi);
                if (restId == null) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("RestList");
            }

            string successMsg = "La Restricción fue agregada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("RestList");
        }


        /* ---------------------------------------------------------------- */
        /* DELETE/RESTORE RESTRICTION */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Restriction
        /// <para> /reservas/restricciones/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRouteRest)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult DeleteRest(string restId)
        {
            if (string.IsNullOrEmpty(restId)) return Error_InvalidUrl();

            try
            {
                var res = BC.DeleteBookRest(restId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            bool isExpired;
            try
            {
                var rest = BC.GetBookRest(restId);
                if (rest == null) isExpired = false;

                if (rest.start_date_hour?.Date > DateTime.Now.Date) isExpired = false;
                else isExpired = true;
            }
            catch
            {
                isExpired = false;
            }


            string successMsg = "La Restricción fue eliminada con éxito";
            SetSuccessMsg(successMsg);


            if (!isExpired) return RedirectToAction("RestList");
            else return RedirectToAction("ExpiredRestList");
        }

        /// <summary>
        /// POST  |  API call to delete a Restriction
        /// <para> /reservas/restricciones/{restId}/restaurar </para>
        /// </summary>
        [HttpGet]
        [Route(restoreRouteRest)]
        [Authorize(Roles = "ADM,TES")]
        public ActionResult RestoreRest(string restId)
        {
            if (string.IsNullOrEmpty(restId)) return Error_InvalidUrl();

            try
            {
                var res = BC.RestoreBookRest(restId);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }


            string successMsg = "La Restricción fue restaurada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("DeletedRestList");
        }


        /* ---------------------------------------------------------------- */
        /* RESCHEDULE DETAILS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to reschedule the data of a Restriction
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADM,SUP,TES")]
        public ActionResult RescheduleRest(BookingRestVM newRest)
        {
            if (newRest == null) return Error_InvalidUrl();

            try
            {
                BookingRestriction apiNewRest = new BookingRestriction();
                apiNewRest = PropertyCopier.Copy(newRest, apiNewRest);

                var res = BC.UpdateBookRest(apiNewRest);
                if (!res)
                {
                    Error_FailedRequest();
                    return RedirectToAction("RestList");
                }
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                Error_CustomError(e.Message);
                return RedirectToAction("RestList");
            }

            string successMsg = "La Restricción fue actualizada con éxito";
            SetSuccessMsg(successMsg);

            return RedirectToAction("RestList");
        }

        public string GetUpdateModalHtml(string restId)
        {
            if (string.IsNullOrEmpty(restId))
            {
                ErrorWriter.InvalidArgumentsError();
                return Resources.Messages.Error_SolicitudFallida;
            }

            string html;

            try
            {
                var item = BC.GetBookRest(restId);

                html = PartialView("Partial/_rescheduleRestModal", item).RenderToString();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Resources.Messages.Error_SolicitudFallida;
            }

            return html;
        }


        /* ---------------------------------------------------------------- OTHERS ---------------------------------------------------------------- */

        /* ---------------------------------------------------------------- */
        /* OTHERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to change the Store Schedule
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADM,SUP,TES")]
        public ActionResult UpdateStoreSche(StoreSchedule model)
        {
            if (model == null) return Error_InvalidForm();

            try
            {
                var res = BC.UpdateStoreSche(model);
                if (!res) return Error_FailedRequest();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            string successMsg = "El Horario de la Tienda fue cambiada";
            SetSuccessMsg(successMsg);

            string referer = GetRefererForError(Request);
            return Redirect(referer);
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
                new NavbarItems("Booking", "BookList", "Reservas"),
                new NavbarItems("Booking", "RestList", "Restricciones de Horario"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }

        // Checkea horas de restricciones vs horas de booking para revisar conflicto.
        public static void CheckTimeConflict_Testing(int restSt, int restEn, int bookSt, int bookEn)
        {
            restSt = 12;
            restEn = 13;
            bookSt = 10;
            bookEn = 13;

            TimeSpan start = new TimeSpan(0, bookSt, 0, 0);
            TimeSpan end = new TimeSpan(0, bookEn, 0, 0);
            TimeSpan ch_start = new TimeSpan(0, restSt, 0, 0);
            TimeSpan ch_end = new TimeSpan(0, restEn, 0, 0);

            int st_st = TimeSpan.Compare(start, ch_start);
            int st_en = TimeSpan.Compare(start, ch_end);
            int en_st = TimeSpan.Compare(end, ch_end);
            int en_en = TimeSpan.Compare(end, ch_start);

            Console.WriteLine(st_st + "  |  " + st_en + "  |  " + en_st + "  |  " + en_en);

            var lst = new List<int>() { st_st, st_en, en_st, en_en };

            var checkMinus = lst.All(x => x <= 0);
            var checkPlus = lst.All(x => x >= 0);

            if (checkMinus || checkPlus) Console.WriteLine("VALIDO >:3c");
            else Debug.WriteLine("NO VALIDO Σ:(c");
        }
    }
}