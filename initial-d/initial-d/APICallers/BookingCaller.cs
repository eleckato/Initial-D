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
        private readonly string prefix = "";
        private readonly string bookPrefix = "";

        List<ReservaVM> mockList = new List<ReservaVM>()
        {
            new ReservaVM()
            {
                
            }
        };


        // TODO Pagination
        /// <summary>
        /// API call to list all Bookings 
        /// </summary>
        public IEnumerable<ReservaVM> GetAllBookings(string status_reserve_id = "", bool deleted = false, string serv_id = "", string appuser_id = "")
        {
            try
            {
                return null;

                //var delString = deleted ? "&deleted=true" : "";
                //var url = $"{bookPrefix}?code={code}&id_cashier={id_cashier}&id_seller={id_seller}&id_appuser={id_appuser}&sale_status_id={sale_status_id}{delString}";

                //// Request Base
                //var request = new RestRequest(url, Method.GET)
                //{
                //    RequestFormat = DataFormat.Json
                //};

                //// Ejecutar request y guardar la respuesta
                //var response = client.Execute<List<SaleVM>>(request);

                //// Levanta una excepción si el status code es diferente de 200
                //CheckStatusCode(response);

                //var sales = response.Data;

                //// Data para conseguir la información más profunda de la venta
                //var saleStatusList = GetAllStatus().ToList();
                //if (saleStatusList == null) return null;

                //var userList = new UsuariosCaller().GetAllUsers(string.Empty, string.Empty, string.Empty, "ACT").ToList();

                //sales.ForEach(sale =>
                //{
                //    sale = ProcessSale(sale, saleStatusList, userList);
                //});

                //// Retorna el producto
                //return sales;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }



    }
}