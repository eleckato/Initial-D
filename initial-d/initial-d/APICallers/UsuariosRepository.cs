using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using initial_d.Common;

namespace initial_d.APICallers
{
    public class UsuariosRepository : RepositorieBase
    {
        private readonly string prefix = "app_users";

        List<Usuario> mockData = new List<Usuario>
        {
            new Usuario()
            {
                appuser_id = "U1",
                username = "cliente_1",
                name = "nombres cl1",
                last_names = "apellido cl1",
                email = "cliente_1@mail.cl",
                adress = "adress cl1",
                lastlogin = new DateTime(2020, 02, 18),
                phone = "+56911111111",
                status_id = "Activo",
                user_type_id = "Cliente",
                birthday = new DateTime(1990, 05, 12),
                mail_confirmed = false,
                created_at = DateTime.Now.AddDays(-10),
                updated_at = DateTime.Now.AddDays(-35),
                deleted = false,
            },
            new Usuario()
            {
                appuser_id = "U2",
                username = "cliente_2",
                name = "nombres cl2",
                last_names = "apellido cl2",
                email = "cliente_1@mail.cl",
                adress = "adress cl2",
                lastlogin = new DateTime(2019, 8, 12),
                phone = "+56922222222",
                status_id = "Banneado",
                user_type_id = "Cliente",
                birthday = new DateTime(2000, 04, 20),
                mail_confirmed = true,
                created_at = DateTime.Now.AddDays(-10),
                updated_at = DateTime.Now.AddDays(-35),
                deleted = false,
            },
        };


        /// <summary>
        /// API call to add an User
        /// </summary>
        /// <param name="newUser"> New User </param>
        public bool AddUser(Usuario newUser)
        {
            if (newUser == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }

        /// <summary>
        /// API call to list all Users
        /// </summary>
        public IEnumerable<Usuario> GetAllUsers()
        {
            //var request = new RestRequest($"{prefix}", Method.GET){ 
            //    RequestFormat = DataFormat.Json
            //};
            //// For pagination
            ////request.AddParameter("page", "1", ParameterType.UrlSegment);
            ////request.AddParameter("size", "1", ParameterType.UrlSegment);

            //var response = client.Execute<List<Usuario>>(request);

            //if (response.Data == null)
            //    throw new Exception(response.ErrorMessage);

            //return response.Data;
            try
            {
                return mockData;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to get an User
        /// </summary>
        /// <param name="userId"> Id of the User </param>
        public Usuario GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                return mockData.SingleOrDefault(x => x.appuser_id.Equals(userId));
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to delete an User
        /// </summary>
        /// <param name="userId"> Id of the User </param>
        public bool DeleteUser(string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                // CALL THE API

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }

        /// <summary>
        /// API call to update an User
        /// </summary>
        /// <param name="newUser"> New User </param>
        public bool UpdateUser(Usuario newUser)
        {
            if (newUser == null)
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                // CALL THE API

                return true;

            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }
    
    }
}