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
                description = "description cl1",
                lastlogin = new DateTime(2020, 02, 18),
                phone = "+56911111111",
                status_id = "1",
                user_type_id = "CLI"
            },
            new Usuario()
            {
                appuser_id = "U2",
                username = "cliente_2",
                name = "nombres cl2",
                last_names = "apellido cl2",
                email = "cliente_1@mail.cl",
                adress = "adress cl2",
                description = "description cl2",
                lastlogin = new DateTime(2019, 8, 12),
                phone = "+56922222222",
                status_id = "1",
                user_type_id = "CLI"
            },
            new Usuario()
            {
                appuser_id = "U4",
                username = "cajero_1",
                name = "nombres caj1",
                last_names = "apellido caj1",
                email = "cajero_1@mail.cl",
                adress = "adress caj1",
                description = "description caj1",
                lastlogin = new DateTime(2020, 4, 2),
                phone = "+56944444444",
                status_id = "1",
                user_type_id = "CAJ"
            },
            new Usuario()
            {
                appuser_id = "U5",
                username = "mecanico_1",
                name = "nombres mec1",
                last_names = "apellido mec1",
                email = "mecanico_1@mail.cl",
                adress = "adress mec1",
                description = "description mec1",
                lastlogin = new DateTime(2020, 2, 12),
                phone = "+56955555555",
                status_id = "1",
                user_type_id = "MEC"
            },
            new Usuario()
            {
                appuser_id = "U6",
                username = "vendedor_1",
                name = "nombres ven1",
                last_names = "apellido ven1",
                email = "vendedor_1@mail.cl",
                adress = "adress ven1",
                description = "description ven1",
                lastlogin = new DateTime(2020, 2, 12),
                phone = "+56966666666",
                status_id = "1",
                user_type_id = "VEN"
            },
            new Usuario()
            {
                appuser_id = "U7",
                username = "supervisor_1",
                name = "nombres sup1",
                last_names = "apellido sup1",
                email = "supervisor_1@mail.cl",
                adress = "adress sup1",
                description = "description sup1",
                lastlogin = new DateTime(2020, 3, 16),
                phone = "+56977777777",
                status_id = "1",
                user_type_id = "SUP"
            },
            new Usuario()
            {
                appuser_id = "U8",
                username = "admin_1",
                name = "nombres adm1",
                last_names = "apellido adm1",
                email = "admin_1@mail.cl",
                adress = "adress adm1",
                description = "description adm1",
                lastlogin = new DateTime(2020, 4, 6),
                phone = "+56988888888",
                status_id = "1",
                user_type_id = "ADM"
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