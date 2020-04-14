using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Security.Claims;

namespace initial_d.APICallers
{
    public class RepositorieBase
    {
        public readonly RestClient client;
        private readonly string _url = ConfigurationManager.AppSettings["BuffetAPI.url"];

        public RepositorieBase()
        {
            client = new RestClient(_url);
            // Get user claims    
            var claims = ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            // Extract the token from there
            var token = claims?.FirstOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/authentication", StringComparison.OrdinalIgnoreCase))?.Value;
            // Add it as a default value in all headers created by this client
            client.AddDefaultHeader("token", token);
        }
    }
}