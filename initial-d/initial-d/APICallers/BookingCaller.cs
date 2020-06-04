using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using initial_d.Common;
using System.Linq;

namespace initial_d.APICallers
{
    public class BookingCaller : CallerBase
    {
        private readonly string prefix = "booking";
        private readonly string bookPrefix = "booking/bookings";
        private readonly string restPrefix = "booking/booking_restrictions";


        // TODO Pagination
        /// <summary>
        /// API call to list all Bookings 
        /// </summary>
        public IEnumerable<BookingVM> GetAllBookings(string status_reserve_id = "", string appuser_id = "", string serv_id = "", bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{bookPrefix}?status_reserve_id={status_reserve_id}&serv_id={serv_id}&appuser_id={appuser_id}{delString}";

                // Request Base
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                // Ejecutar request y guardar la respuesta
                var response = client.Execute<List<BookingVM>>(request);

                // Levanta una excepción si el status code es diferente de 200
                CheckStatusCode(response);

                var bookings = response.Data;

                // Data para conseguir la información más profunda de la venta
                var bookStatusList = GetAllBookStatus().ToList();
                if (bookStatusList == null) return null;

                var userList = new UsuariosCaller().GetAllUsers(string.Empty, string.Empty, string.Empty, "ACT").ToList();
                if (userList == null) return null;

                var servList = new ServiciosCaller().GetAllServ(string.Empty, "ACT").ToList();
                if (servList == null) return null;


                bookings.ForEach(book =>
                {
                    book = ProcessBook(book, bookStatusList, userList, servList);
                });

                // Retorna el producto
                return bookings;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get a Booking
        /// </summary>
        /// <param name="bookId"> Booking Id </param>
        public BookingVM GetBook(string bookId)
        {
            if (string.IsNullOrEmpty(bookId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{bookPrefix}/{bookId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<BookingVM>(request);

                string notFoundMsg = "La Reserva requerida no existe";
                CheckStatusCode(response, notFoundMsg);


                var book = response.Data;

                var bookStatusList = GetAllBookStatus().ToList();
                if (bookStatusList == null) return null;

                var userList = new UsuariosCaller().GetAllUsers(string.Empty, string.Empty, string.Empty, "ACT").ToList();
                if (userList == null) return null;

                var servList = new ServiciosCaller().GetAllServ(string.Empty, "ACT").ToList();
                if (servList == null) return null;

                book = ProcessBook(book, bookStatusList, userList, servList);

                return book;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        // TODO API CALL
        /// <summary>
        /// API call to add a Booking
        /// </summary>
        /// <param name="newRest"> New Booking </param>
        public string AddBooking(BookingVM newBook)
        {
            if (newBook == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                return "7087D8C3A17F48FF961A23EB4575B519";

                //var request = new RestRequest($"{bookPrefix}", Method.POST)
                //{
                //    RequestFormat = DataFormat.Json
                //};

                //request.AddJsonBody(newBook);

                //var response = client.Execute(request);

                //CheckStatusCode(response);

                //return response.Content;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to update a Booking
        /// </summary>
        /// <param name="newBook"> New Booking </param>
        public bool UpdateBooking(Booking newBook)
        {
            if (newBook == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                newBook.updated_at = DateTime.Now;

                var bookId = newBook.booking_id;

                var request = new RestRequest($"{bookPrefix}/{bookId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newBook);

                var response = client.Execute(request);

                string notFoundMsg = "La Reserva requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to delete a Booking
        /// </summary>
        /// <param name="bookId"> Booking Id </param>
        public bool DeleteBooking(string bookId)
        {
            if (string.IsNullOrEmpty(bookId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{bookPrefix}/{bookId}", Method.DELETE);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        //TODO API CALL. COMPROBAR URL
        /// <summary>
        /// API call to restore a Booking
        /// </summary>
        /// <param name="bookId"> Booking Id </param>
        public bool RestoreBooking(string bookId)
        {
            if (string.IsNullOrEmpty(bookId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                return true;

                var request = new RestRequest($"{bookPrefix}/{bookId}/restore", Method.PUT);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to change the Status of a Booking
        /// </summary>
        /// <param name="bookId"> Booking Id </param>
        /// <param name="bookStatusId"> Booking Status Id </param>
        public bool ChangeBookStatus(string bookId, string bookStatusId)
        {
            if (string.IsNullOrEmpty(bookId) || string.IsNullOrEmpty(bookStatusId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{bookPrefix}/{bookId}/change-status?status={bookStatusId}", Method.POST);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* BOOKING RESTRICTION */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// API call to list all Bookings Restrictions
        /// </summary>
        public IEnumerable<BookingRestVM> GetAllBookRest(string serv_id = "", bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{restPrefix}?serv_id={serv_id}{delString}";

                // Request Base
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                // Ejecutar request y guardar la respuesta
                var response = client.Execute<List<BookingRestVM>>(request);

                // Levanta una excepción si el status code es diferente de 200
                CheckStatusCode(response);

                var rests = response.Data;

                var servList = new ServiciosCaller().GetAllServ(string.Empty, "ACT").ToList();
                if (servList == null) return null;

                rests.ForEach(rest =>
                {
                    rest = ProcessRest(rest, servList);
                });

                // Retorna el producto
                return rests;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get a Booking Restriction
        /// </summary>
        /// <param name="restId"> Restriction Id </param>
        public BookingRestVM GetBookRest(string restId)
        {
            if (string.IsNullOrEmpty(restId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{restPrefix}/{restId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<BookingRestVM>(request);

                string notFoundMsg = "La Reserva requerida no existe";
                CheckStatusCode(response, notFoundMsg);


                var rest = response.Data;

                var servList = new ServiciosCaller().GetAllServ(string.Empty, "ACT").ToList();
                if (servList == null) return null;

                rest = ProcessRest(rest, servList);

                return rest;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        // TODO API CALL
        /// <summary>
        /// API call to add a Booking Restriction
        /// </summary>
        /// <param name="newRest"> New Booking Restriction</param>
        public string AddBookRest(BookingRestVM newRest)
        {
            if (newRest == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                return "DCD286FE94C045669CBD7B921A3EEE71";

                //var request = new RestRequest($"{restPrefix}", Method.POST)
                //{
                //    RequestFormat = DataFormat.Json
                //};

                //request.AddJsonBody(newRest);

                //var response = client.Execute(request);

                //CheckStatusCode(response);

                //return response.Content;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to update a Booking Restriction
        /// </summary>
        /// <param name="newRest"> New Booking Restriction </param>
        public bool UpdateBookRest(BookingRestVM newRest)
        {
            if (newRest == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var restId = newRest.restriction_id;

                var request = new RestRequest($"{restPrefix}/{restId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newRest);

                var response = client.Execute(request);

                string notFoundMsg = "La Restricción de Reserva requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to delete a Booking Restriction
        /// </summary>
        /// <param name="restId"> Booking Restriction Id </param>
        public bool DeleteBookRest(string restId)
        {
            if (string.IsNullOrEmpty(restId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{restPrefix}/{restId}", Method.DELETE);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        //TODO API CALL. COMPROBAR URL
        /// <summary>
        /// API call to restore a Booking Restriction
        /// </summary>
        /// <param name="restId"> Booking Restriction Id </param>
        public bool RestoreBookRest(string restId)
        {
            if (string.IsNullOrEmpty(restId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                return true;

                var request = new RestRequest($"{restPrefix}/{restId}/restore", Method.PUT);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* STORE SCHEDULE */
        /* ---------------------------------------------------------------- */

        //TODO QUE LA API DEVUELVA UN SOLO OBJETO EN VEZ DE UNA LISTA
        /// <summary>
        /// API call to get the Store Schedule
        /// </summary>
        public StoreSchedule GetStoreSche()
        {
            try
            {
                var request = new RestRequest($"{prefix}/store_schedule", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<StoreSchedule>>(request);

                string notFoundMsg = "No se pudo encontrar el horario de la tienda, contacte a soporte.";
                CheckStatusCode(response, notFoundMsg);

                var data = response.Data;

                return response.Data.FirstOrDefault();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        //TODO API CALL
        public bool UpdateStoreSche(StoreSchedule model)
        {
            if (model == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                model.updated_at = DateTime.Now;

                return true;

                ////var bookId = newBook.booking_id;

                ////var request = new RestRequest($"{bookPrefix}/{bookId}", Method.POST)
                ////{
                ////    RequestFormat = DataFormat.Json
                ////};

                ////request.AddJsonBody(newBook);

                ////var response = client.Execute(request);

                ////string notFoundMsg = "La Reserva requerida no existe";
                ////CheckStatusCode(response, notFoundMsg);

                ////return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* GET SECONDARY DATA */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to get all Booking Status, with name and ID
        /// </summary>
        public IEnumerable<BookingStatus> GetAllBookStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/status_booking", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<BookingStatus>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }


        /* ---------------------------------------------------------------- */
        /* HELPERS */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// Set all the secondary data ,like getting the status Name from the status Id
        /// </summary>
        /// <param name="book"> Booking to process </param>
        /// <param name="bookStatusList"> List with all Booking Status </param>
        public BookingVM ProcessBook(BookingVM book, List<BookingStatus> bookStatusList, List<Usuario> userList, List<Servicio> servList)
        {
            if (book == null || bookStatusList == null) return null;

            // Set Status
            var thisBookStatus = bookStatusList.FirstOrDefault(status => status.status_booking_id.Equals(book.status_booking_id));
            book.statusName = thisBookStatus?.name ?? "-";

            // Set User
            var thisUser = userList.FirstOrDefault(user => user.appuser_id.Equals(book.appuser_id));
            book.user = thisUser ?? null;

            var thisServ = servList.FirstOrDefault(serv => serv.serv_id.Equals(serv.serv_id));
            book.serv = thisServ ?? null;

            return book;
        }

        
        /// <summary>
        /// Set all the secondary data ,like getting the status Name from the status Id
        /// </summary>
        /// <param name="rest"> Booking to process </param>
        /// <param name="servList"> List with all Services </param>
        public BookingRestVM ProcessRest(BookingRestVM rest, List<Servicio> servList)
        {
            if (rest == null || servList == null) return null;

            var thisServ = servList.FirstOrDefault(serv => serv.serv_id.Equals(serv.serv_id));
            rest.serv = thisServ ?? null;

            return rest;
        }

    }
}