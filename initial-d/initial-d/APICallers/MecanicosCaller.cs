using initial_d.Common;
using initial_d.Models.APIModels;
using initial_d.Providers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace initial_d.APICallers
{
    public class MecanicosCaller : CallerBase
    {
        private readonly string prefix = "mech-adm";


        List<Mecanico> mockData = new List<Mecanico>
        {
            new Mecanico()
            {
                appuser_id = "MEC1",
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
                appuser_id = "MEC2",
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

        /* ---------------------------------------------------------------- */
        /* MECHANIC CRUD */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to add an Mechanic
        /// </summary>
        /// <param name="newMech"> New Mechanic model with the data </param>
        public string AddMech(Usuario newMech)
        {
            if (newMech == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var userId = newMech.appuser_id;

                var request = new RestRequest("user-auth/register", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                newMech.hash = JwtProvider.EncryptHMAC(newMech.hash);
                request.AddJsonBody(newMech);

                var response = client.Execute(request);

                if (response.StatusCode == HttpStatusCode.Conflict)
                    throw new Exception("El Nombre de Usuario o Mail ya existe");

                CheckStatusCode(response);

                return response.Content;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        // TODO Pagination
        /// <summary>
        /// API call to list all Mechanics
        /// </summary>
        public IEnumerable<Mecanico> GetAllMech(string userName, string userRut, string userStatusId, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{prefix}/mechanics?username={userName}&rut={userRut}&status_id={userStatusId}{delString}";

                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<Mecanico>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get a Mechanic
        /// </summary>
        /// <param name="mechId"> Mechanic Id </param>
        public Mecanico GetMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{prefix}/mechanics/{mechId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Mecanico>(request);

                string notFoundMsg = "El Mecánico requerido no existe";
                CheckStatusCode(response, notFoundMsg);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to update an Mechanic
        /// </summary>
        /// <param name="newMech"> New Mechanic model with the data </param>
        public bool UpdateMech(Usuario newMech)
        {
            if (newMech == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var userId = newMech.appuser_id;

                var request = new RestRequest($"{prefix}/mechanics/{userId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newMech);

                var response = client.Execute(request);

                string notFoundMsg = "El Mecánico requerido no existe";
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
        /// API call to delete a Mechanic
        /// </summary>
        /// <param name="mechId"> Mechanic Id </param>
        public bool DeleteMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{prefix}/mechanics/{mechId}", Method.DELETE);

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
        /// API call to restore an Mechanic
        /// </summary>
        /// <param name="mechId"> Mechanic Id </param>
        public bool RestoreMech(string mechId)
        {
            if (string.IsNullOrEmpty(mechId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{prefix}/mechanics/{mechId}/restore", Method.PUT);

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
        /// API call to change the Status of a Mechanic
        /// </summary>
        /// <param name="mechId"> User Id </param>
        /// <param name="statusId"> User Status Id </param>
        public bool ChangeMechStatus(string mechId, string statusId)
        {
            if (string.IsNullOrEmpty(mechId) || string.IsNullOrEmpty(statusId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                string url = $"{prefix}/mechanics/{mechId}/change-status?status={statusId}";

                var request = new RestRequest(url, Method.POST);

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
    }
}