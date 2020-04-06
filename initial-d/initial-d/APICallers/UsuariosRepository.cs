using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.APICallers
{
    public class UsuariosRepository : RepositorieBase
    {
        private readonly string prefix = "app_users";

        /// <summary>
        /// Get a list with all the Users 
        /// </summary>
        public IEnumerable<Usuario> GetAllUsers()
        {
            var request = new RestRequest($"{prefix}", Method.GET){ 
                RequestFormat = DataFormat.Json
            };
            //request.AddParameter("page", "1", ParameterType.UrlSegment);
            //request.AddParameter("size", "1", ParameterType.UrlSegment);

            var response = client.Execute<List<Usuario>>(request);

            if (response.Data == null)
                throw new Exception(response.ErrorMessage);

            return response.Data;
        }

    }
}