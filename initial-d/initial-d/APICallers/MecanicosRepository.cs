using initial_d.Common;
using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.APICallers
{
    public class MecanicosRepository : RepositorieBase
    {
        private readonly string prefix = "mech-adm";


        List<Mecanico> mockData = new List<Mecanico>
        {
            new Mecanico()
            {
                appuser_id = "U1",
                username = "cliente_1",
                name = "nombres cl1",
                last_names = "apellido cl1",
                email = "cliente_1@mail.cl",
                adress = "address cl1",
                lastlogin = new DateTime(2020, 02, 18),
                phone = "+56911111111",
                status_id = "ACT",
                user_type_id = "MEC",
                birthday = new DateTime(1990, 05, 12),
                mail_confirmed = false,
                created_at = DateTime.Now.AddDays(-10),
                updated_at = DateTime.Now.AddDays(-35),
                deleted = false,
                hasDebt = false,
                hasPendingPublication = true,
            },
            new Mecanico()
            {
                appuser_id = "U2",
                username = "cliente_2",
                name = "nombres cl2",
                last_names = "apellido cl2",
                email = "cliente_1@mail.cl",
                adress = "address cl2",
                lastlogin = new DateTime(2019, 8, 12),
                phone = "+56922222222",
                status_id = "ACT",
                user_type_id = "MEC",
                birthday = new DateTime(2000, 04, 20),
                mail_confirmed = true,
                created_at = DateTime.Now.AddDays(-10),
                updated_at = DateTime.Now.AddDays(-35),
                deleted = false,
                hasDebt = true,
                hasPendingPublication = false,
            },
        };


        // TODO Connection with API
        // TODO Search filters
        // TODO Pagination
        /// <summary>
        /// API call to list all Mechanics
        /// </summary>
        public IEnumerable<Mecanico> GetAllMech()
        {
            try
            {
                //var request = new RestRequest($"{prefix}/mechanics", Method.GET)
                //{
                //    RequestFormat = DataFormat.Json
                //};
                //// For pagination
                ////request.AddParameter("page", "1", ParameterType.UrlSegment);
                ////request.AddParameter("size", "1", ParameterType.UrlSegment);

                //var response = client.Execute<List<Mecanico>>(request);

                //CheckStatusCode(response);

                //return response.Data;

                return mockData;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }



    }
}