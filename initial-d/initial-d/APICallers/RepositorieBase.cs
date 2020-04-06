using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace initial_d.APICallers
{
    public class RepositorieBase
    {
        public readonly RestClient client;
        private readonly string _url = ConfigurationManager.AppSettings["BuffetAPI.url"];

        public RepositorieBase()
        {
            client = new RestClient(_url);
        }
    }
}