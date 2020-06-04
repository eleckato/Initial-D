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
    [RoutePrefix("reservas")]
    public class BookingController : BaseController
    {
        readonly BookingCaller BC = new BookingCaller();
        readonly UsuariosCaller UC = new UsuariosCaller();

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


                bookList = BC.GetAllBookings(statusId, userId, deleted : true).ToList();
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
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return Error_CustomError(e.Message);
            }

            return View(book);
        }


        /////* UPDATE BOOKING */
        /////* ---------------------------------------------------------------- */

        /////// <summary>
        /////// GET  |  Show a form to update an existing Booking
        /////// <para> /reservas/{id}/actualizar </para>
        /////// </summary>
        ////[HttpGet]
        ////[Route(updateRoute)]
        ////public ActionResult UpdateBook(string bookId)
        ////{
        ////    if (string.IsNullOrEmpty(bookId)) return Error_InvalidUrl();

        ////    BookingVM book;
        ////    List<BookingStatus> bookStatusList;

        ////    try
        ////    {
        ////        book = BC.GetBook(bookId);
        ////        if (book == null) return Error_FailedRequest();

        ////        bookStatusList = BC.GetAllBookStatus().ToList();
        ////        if (bookStatusList == null) return Error_FailedRequest();
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        ErrorWriter.ExceptionError(e);
        ////        return Error_CustomError(e.Message);
        ////    }

        ////    ViewBag.bookStatusLst = new SelectList(bookStatusList, "status_booking_id", "name", book.status_booking_id);

        ////    return View(book);
        ////}

        /////// <summary>
        /////// POST  |  API call to update the data of a Booking
        /////// <para> /reservas/{id}/actualizar </para>
        /////// </summary>
        ////[HttpPost]
        ////[Route(updateRoute)]
        ////public ActionResult UpdateBook(BookingVM newBook)
        ////{
        ////    if (newBook == null) return Error_InvalidUrl();
        ////    string bookId = newBook.booking_id;

        ////    try
        ////    {
        ////        Booking apiNewBook = newBook;

        ////        var res = BC.UpdateBooking(apiNewBook);

        ////        if (!res)
        ////        {
        ////            Error_FailedRequest();
        ////            return RedirectToAction("UpdateBook", new { bookId });
        ////        }
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        ErrorWriter.ExceptionError(e);
        ////        Error_CustomError(e.Message);
        ////        return RedirectToAction("UpdateBook", new { bookId });
        ////    }

        ////    string successMsg = "La Reserva fue actualizada con éxito";
        ////    SetSuccessMsg(successMsg);

        ////    return RedirectToAction("BookDetails", new { bookId });
        ////}

        /* ---------------------------------------------------------------- */
        /* DELETE BOOKING */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// POST  |  API call to delete a Booking
        /// <para> /reservas/{id}/eliminar </para>
        /// </summary>
        [HttpGet]
        [Route(deleteRoute)]
        public ActionResult DeleteBook(string bookid)
        {
            if (string.IsNullOrEmpty(bookid)) return Error_InvalidUrl();

            try
            {
                var canRes = BC.ChangeBookStatus(bookid, "CAN");
                if (!canRes) return Error_FailedRequest();

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


        /* ---------------------------------------------------------------- */
        /* OTHER ACTIONS */
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


        /// <summary>
        /// POST  |  API call to change the Store Schedule
        /// </summary>
        [HttpPost]
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
        /// Set and saves the data for the navbar in a ViewBag
        /// </summary>
        private void SetNavbar()
        {
            List<NavbarItems> InternalNavbar = new List<NavbarItems>()
            {
                new NavbarItems("Reserva", "RestList", "Reservas"),
            };

            ViewBag.InternalNavbar = InternalNavbar;
        }
    }
}