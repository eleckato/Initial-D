using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using initial_d.Common;
using System.Net;
using System.Linq;

namespace initial_d.APICallers
{
    public class UsuariosRepository : RepositorieBase
    {
        private readonly string prefix = "user-adm";

        List<Usuario> mockData = new List<Usuario>
        {
            new Usuario()
            {
                appuser_id = "U1",
                username = "cliente_1",
                name = "nombres cl1",
                last_names = "apellido cl1",
                email = "cliente_1@mail.cl",
                adress = "address cl1",
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
                adress = "address cl2",
                lastlogin = new DateTime(2019, 8, 12),
                phone = "+56922222222",
                status_id = "Banned",
                user_type_id = "Cliente",
                birthday = new DateTime(2000, 04, 20),
                mail_confirmed = true,
                created_at = DateTime.Now.AddDays(-10),
                updated_at = DateTime.Now.AddDays(-35),
                deleted = false,
            },
        };

        // TODO Connection with API
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

        // TODO Search filters
        // TODO Pagination
        /// <summary>
        /// API call to list all Users
        /// </summary>
        public IEnumerable<Usuario> GetAllUsers()
        {
            try
            {
                var request = new RestRequest($"{prefix}/users", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };
                // For pagination
                //request.AddParameter("page", "1", ParameterType.UrlSegment);
                //request.AddParameter("size", "1", ParameterType.UrlSegment);

                var response = client.Execute<List<Usuario>>(request);

                CheckStatusCode(response);

                return response.Data;

                //// return mockData;
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
                var request = new RestRequest($"{prefix}/users/{userId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<Usuario>(request);

                string notFoundMsg = "El Usuario requerido no existe";
                CheckStatusCode(response, notFoundMsg);

                return response.Data;

                //// return mockData.SingleOrDefault(x => x.appuser_id.Equals(userId));
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        // TODO Connection with API
        /// <summary>
        /// API call to delete an User
        /// </summary>
        /// <param name="userId"> Id of the User </param>
        public bool DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
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

        // TODO Connection with API
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
    
        /// <summary>
        /// API call to get all User Types, with name and ID
        /// </summary>
        public IEnumerable<UserType> GetAllTypes()
        {
            try
            {
                var request = new RestRequest($"{prefix}/user-type", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<UserType>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to get all User Status, with name and ID
        /// </summary>
        public IEnumerable<UserStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/user-status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<UserStatus>>(request);

                CheckStatusCode(response);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }
    
        /// <summary>
        /// Set all the secondary data ,like getting the status Name from the status Id
        /// </summary>
        /// <param name="user"> User to process </param>
        /// <param name="userTypeLst"> List with all User Types </param>
        /// <param name="userStatusLst"> List with all User Status </param>
        public Usuario ProcessUser(Usuario user, List<UserType> userTypeLst, List<UserStatus> userStatusLst)
        {
            if (user == null || userTypeLst == null || userStatusLst == null) return null;

            var thisUserType = userTypeLst.FirstOrDefault(type => type.user_type_id.Equals(user.user_type_id));
            user.user_type_name = thisUserType?.name ?? string.Empty;
            var thisUserStatus = userStatusLst.FirstOrDefault(status => status.status_id.Equals(user.status_id));
            user.status_name = thisUserStatus?.status ?? string.Empty;

            return user;
        }
        
    }
}