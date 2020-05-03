﻿using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using initial_d.Common;
using System.Linq;

namespace initial_d.APICallers
{
    public class ServiciosCaller : CallerBase
    {
        private readonly string prefix = "supply-adm";
        private readonly string fullPrefix = "supply-adm/service";

        /* ---------------------------------------------------------------- */
        /* SERVICES CALLER */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// API call to list all Services
        /// </summary>
        public IEnumerable<Servicio> GetAllServ(string name, string serv_status, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                var url = $"{fullPrefix}?name={name}&serv_status={serv_status}{delString}";

                // Request Base
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                // Ejecutar request y guardar la respuesta
                var response = client.Execute<List<Servicio>>(request);

                // Levanta una excepción si el status code es diferente de 200
                CheckStatusCode(response);

                // Retorna el producto
                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to get a Service
        /// </summary>
        /// <param name="servId"> Service Id </param>
        public Servicio GetServ(string servId)
        {
            if (string.IsNullOrEmpty(servId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}/{servId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Servicio>(request);

                string notFoundMsg = "El Servicio requerido no existe";
                CheckStatusCode(response, notFoundMsg);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        // TODO API Call
        /// <summary>
        /// API call to add a Service
        /// </summary>
        /// <param name="newServ"> New Service </param>
        public string AddServ(Servicio newServ)
        {
            if (newServ == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newServ);

                var response = client.Execute(request);

                CheckStatusCode(response);

                return response.Content;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                throw e;
            }
        }

        /// <summary>
        /// API call to update a Service
        /// </summary>
        /// <param name="newServ"> New Service </param>
        public bool UpdateServ(Servicio newServ)
        {
            if (newServ == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var servId = newServ.serv_id;

                var request = new RestRequest($"{fullPrefix}/{servId}", Method.POST)
                {
                    RequestFormat = DataFormat.Json
                };

                request.AddJsonBody(newServ);

                var response = client.Execute(request);

                string notFoundMsg = "El Servicio requerido no existe";
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
        /// API call to delete a Service
        /// </summary>
        /// <param name="servId"> Service Id </param>
        public bool DeleteServ(string servId)
        {
            if (string.IsNullOrEmpty(servId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}/{servId}", Method.DELETE);

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
        /// API call to restore a Service
        /// </summary>
        /// <param name="servId"> Service Id </param>
        public bool RestoreServ(string servId)
        {
            if (string.IsNullOrEmpty(servId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}/{servId}/restore", Method.PUT);

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
        /// API call to change the Status of a Service
        /// </summary>
        /// <param name="servId"> Service Id </param>
        /// <param name="servStatusId"> Service Status Id </param>
        public bool ChangeServStatus(string servId, string servStatusId)
        {
            if (string.IsNullOrEmpty(servId) || string.IsNullOrEmpty(servStatusId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{fullPrefix}/{servId}/change-status?serv_status={servStatusId}", Method.POST);

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
        /* GET SECONDARY DATA */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to get all Service Status, with name and ID
        /// </summary>
        public IEnumerable<ServStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/serv_status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<ServStatus>>(request);

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
        /// <param name="serv"> Service to process </param>
        /// <param name="servStatusList"> List with all Service Status </param>
        public Servicio ProcessServ(Servicio serv, List<ServStatus> servStatusList)
        {
            if (serv == null || servStatusList == null) return null;

            // Set Status
            var thisProdStatus = servStatusList.FirstOrDefault(status => status.status_id.Equals(serv.serv_status));
            serv.status_name = thisProdStatus?.status ?? string.Empty;

            return serv;
        }

    }
}