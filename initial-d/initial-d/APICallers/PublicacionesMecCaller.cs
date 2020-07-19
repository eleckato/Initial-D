using initial_d.Common;
using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace initial_d.APICallers
{
    public class PublicacionesMecCaller : CallerBase
    {
        private readonly string prefix = "pub-adm";

        List<PublicacionMec> mockData = new List<PublicacionMec>()
        {
            new PublicacionMec()
            {
                public_id = "PUB1",
                appuser_id = "MEC1",
                public_status_id = "PEN",
                created_at = DateTime.Today,
                updated_at = DateTime.Today,
                deleted = true,
                title = "Lorem ipsum dolor sit amet",
                public_desc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. \nMaecenas feugiat quam tempor, suscipit tortor non\nmas dolor sit amet lorem ipsum etc etc",
                services = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas feugiat quam tempor, suscipit tortor non, cursus urna. Suspendisse felis risus, finibus non ante at, fermentum interdum dolor. Proin scelerisque fermentum mattis. Aenean non imperdiet est. Nullam ac nisl quis odio pharetra varius id a elit. Quisque arcu urna, convallis in metus vitae, eleifend vestibulum est. Praesent aliquam ac sem vel ornare.\n" +
                "Donec sit amet quam placerat, aliquam arcu a, lobortis libero. Morbi commodo, leo finibus condimentum rhoncus, sapien arcu molestie justo, at consectetur nunc mi non magna. Sed pharetra fringilla justo, vitae placerat justo tempus ac. Praesent congue purus sed sapien suscipit pellentesque. Aliquam rhoncus eros sit amet urna accumsan volutpat. Curabitur at odio mi. Sed commodo nec lacus sed dignissim.\n",
                schedule = "Horario Lorem ipsum dolor sit amet",
                bussiness_name = "Nombre negocio",
                address = "Calle Tuerca #666",
                comuna = "Melipilla",
                region = "Región Metropolitana",
                landline = null,
                mobile_number = "+569123456789",
                email = "email_negocio_1@gmail.com",
                views = 125
            },
            new PublicacionMec()
            {
                public_id = "PUB2",
                appuser_id = "MEC1",
                public_status_id = "INA",
                created_at = DateTime.Today,
                updated_at = DateTime.Today,
                deleted = false,
                title = "Lorem ipsum dolor sit amet",
                public_desc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas feugiat quam tempor, suscipit tortor non, cursus urna. Suspendisse felis risus, finibus non ante at, fermentum interdum dolor. Proin scelerisque fermentum mattis. Aenean non imperdiet est. Nullam ac nisl quis odio pharetra varius id a elit. Quisque arcu urna, convallis in metus vitae, eleifend vestibulum est. Praesent aliquam ac sem vel ornare.\n" +
                "Donec sit amet quam placerat, aliquam arcu a, lobortis libero. Morbi commodo, leo finibus condimentum rhoncus, sapien arcu molestie justo, at consectetur nunc mi non magna. Sed pharetra fringilla justo, vitae placerat justo tempus ac. Praesent congue purus sed sapien suscipit pellentesque. Aliquam rhoncus eros sit amet urna accumsan volutpat. Curabitur at odio mi. Sed commodo nec lacus sed dignissim.\n" +
                "Curabitur interdum dapibus arcu, vel aliquam nunc hendrerit ac. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin eget sapien quis lacus bibendum fringilla. Sed sit amet diam bibendum, tempus est a, aliquam nunc. Interdum et malesuada fames ac ante ipsum primis in faucibus. Maecenas accumsan tellus id purus hendrerit, in facilisis neque semper. Pellentesque rhoncus est non purus semper, eget imperdiet leo suscipit. In iaculis vitae ex vel maximus. In sed leo in nulla finibus dignissim. Suspendisse non. ",
                services = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas feugiat quam tempor, suscipit tortor non, cursus urna. Suspendisse felis risus, finibus non ante at, fermentum interdum dolor. Proin scelerisque fermentum mattis. Aenean non imperdiet est. Nullam ac nisl quis odio pharetra varius id a elit. Quisque arcu urna, convallis in metus vitae, eleifend vestibulum est. Praesent aliquam ac sem vel ornare.\n" +
                "Donec sit amet quam placerat, aliquam arcu a, lobortis libero. Morbi commodo, leo finibus condimentum rhoncus, sapien arcu molestie justo, at consectetur nunc mi non magna. Sed pharetra fringilla justo, vitae placerat justo tempus ac. Praesent congue purus sed sapien suscipit pellentesque. Aliquam rhoncus eros sit amet urna accumsan volutpat. Curabitur at odio mi. Sed commodo nec lacus sed dignissim.\n",
                schedule = "Horario Lorem ipsum dolor sit amet",
                bussiness_name = "Nombre negocio 2",
                address = "Otra Calle Más #420",
                comuna = "Talagante",
                region = "Región Metropolitana",
                landline = "028321364",
                mobile_number = "+569123456789",
                email = "email_negocio_1@gmail.com",
                views = 12
            },
            new PublicacionMec()
            {
                public_id = "PUB3",
                appuser_id = "MEC2",
                public_status_id = "ACT",
                created_at = DateTime.Today,
                updated_at = DateTime.Today,
                deleted = false,
                title = "Lorem ipsum dolor sit amet",
                public_desc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas feugiat quam tempor, suscipit tortor non, cursus urna. Suspendisse felis risus, finibus non ante at, fermentum interdum dolor. Proin scelerisque fermentum mattis. Aenean non imperdiet est. Nullam ac nisl quis odio pharetra varius id a elit. Quisque arcu urna, convallis in metus vitae, eleifend vestibulum est. Praesent aliquam ac sem vel ornare.\n" +
                "Donec sit amet quam placerat, aliquam arcu a, lobortis libero. Morbi commodo, leo finibus condimentum rhoncus, sapien arcu molestie justo, at consectetur nunc mi non magna. Sed pharetra fringilla justo, vitae placerat justo tempus ac. Praesent congue purus sed sapien suscipit pellentesque. Aliquam rhoncus eros sit amet urna accumsan volutpat. Curabitur at odio mi. Sed commodo nec lacus sed dignissim.\n" +
                "Curabitur interdum dapibus arcu, vel aliquam nunc hendrerit ac. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin eget sapien quis lacus bibendum fringilla. Sed sit amet diam bibendum, tempus est a, aliquam nunc. Interdum et malesuada fames ac ante ipsum primis in faucibus. Maecenas accumsan tellus id purus hendrerit, in facilisis neque semper. Pellentesque rhoncus est non purus semper, eget imperdiet leo suscipit. In iaculis vitae ex vel maximus. In sed leo in nulla finibus dignissim. Suspendisse non. ",
                services = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas feugiat quam tempor, suscipit tortor non, cursus urna. Suspendisse felis risus, finibus non ante at, fermentum interdum dolor. Proin scelerisque fermentum mattis. Aenean non imperdiet est. Nullam ac nisl quis odio pharetra varius id a elit. Quisque arcu urna, convallis in metus vitae, eleifend vestibulum est. Praesent aliquam ac sem vel ornare.\n" +
                "Donec sit amet quam placerat, aliquam arcu a, lobortis libero. Morbi commodo, leo finibus condimentum rhoncus, sapien arcu molestie justo, at consectetur nunc mi non magna. Sed pharetra fringilla justo, vitae placerat justo tempus ac. Praesent congue purus sed sapien suscipit pellentesque. Aliquam rhoncus eros sit amet urna accumsan volutpat. Curabitur at odio mi. Sed commodo nec lacus sed dignissim.\n",
                schedule = "Horario Lorem ipsum dolor sit amet",
                bussiness_name = null,
                address = "Calle Perno #69",
                comuna = "Melipilla",
                region = "Región Metropolitana",
                landline = null,
                mobile_number = "+569123456789",
                email = "email_negocio_2@gmail.com",
                views = 666
            }
        };

        /* ---------------------------------------------------------------- */
        /* PUBLICATION CRUD */
        /* ---------------------------------------------------------------- */

        // TODO Pagination
        /// <summary>
        /// API call to list all the Mechanic Publications
        /// </summary>
        public IEnumerable<PublicacionMec> GetAllPub(string comuna, string statusId, string bussName, string title, string userId, bool deleted = false)
        {
            try
            {
                var delString = deleted ? "&deleted=true" : "";
                string url = $"{prefix}/publications?comuna={comuna}&public_status_id={statusId}&bussiness_name={bussName}&title={title}&user_id={userId}{delString}";
                var request = new RestRequest(url, Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<PublicacionMec>>(request);

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
        /// API call to get a Mechanic Publication
        /// </summary>
        /// <param name="pubId"> Publication Id </param>
        public PublicacionMec GetPub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId))
            {
                ErrorWriter.InvalidArgumentsError();
                return null;
            }

            try
            {
                var request = new RestRequest($"{prefix}/publications/{pubId}", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<PublicacionMec>(request);

                string notFoundMsg = "La Publicación requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                return response.Data;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }

        /// <summary>
        /// API call to delete a Mechanic Publication
        /// </summary>
        /// <param name="pubId"> Publication Id </param>
        public bool DeletePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{prefix}/publications/{pubId}", Method.DELETE)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                string notFoundMsg = "La Publicación requerida no existe";
                CheckStatusCode(response, notFoundMsg);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }

        /// <summary>
        /// API call to restore an Publication
        /// </summary>
        /// <param name="pubId"> Publication Id </param>
        public bool RestorePub(string pubId)
        {
            if (string.IsNullOrEmpty(pubId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                var request = new RestRequest($"{prefix}/publications/{pubId}/restore", Method.PUT);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

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
        /// API call to update a Mechanic Publication
        /// </summary>
        /// <param name="newPub"> New Publication model with the data </param>
        public bool UpdatePub(PublicacionMec newPub)
        {
            if (newPub == null)
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
        /// API call to update the Status of a Mechanic Publication
        /// </summary>
        /// <param name="pubId"> Publication Id </param>
        /// <param name="newStateId"> Id of the new Status for the Publication </param>
        public bool ChangeStatus(string pubId, string newStateId)
        {
            if (string.IsNullOrEmpty(pubId) || string.IsNullOrEmpty(newStateId))
            {
                ErrorWriter.InvalidArgumentsError();
                return false;
            }

            try
            {
                string url = $"{prefix}/publications/{pubId}/change-status?public_status_id={newStateId}";

                var request = new RestRequest(url, Method.POST);

                var response = client.Execute(request);

                // Throw an exception if the StatusCode is different from 200
                CheckStatusCode(response);

                return true;
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return false;
            }
        }


        /* ---------------------------------------------------------------- */
        /* GET SECONDARY DATA */
        /* ---------------------------------------------------------------- */

        /// <summary>
        /// API call to get all Publication Status, with name and ID
        /// </summary>
        public IEnumerable<PublicStatus> GetAllStatus()
        {
            try
            {
                var request = new RestRequest($"{prefix}/public-status", Method.GET)
                {
                    RequestFormat = DataFormat.Json
                };

                var response = client.Execute<List<PublicStatus>>(request);

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
        /// <param name="pub"> Publication to process </param>
        /// <param name="pubStatusLst"> List with all Status Names </param>
        public PublicacionMec ProcessPub(PublicacionMec pub, List<PublicStatus> pubStatusLst, List<Mecanico> mechList)
        {
            if (pub == null || pubStatusLst == null) return null;

            var thisUserType = pubStatusLst.FirstOrDefault(type => type.public_status_id.Equals(pub.public_status_id));
            pub.status_name = thisUserType?.status_name ?? string.Empty;

            var thisMech = mechList.FirstOrDefault(x => x.appuser_id.Equals(pub.appuser_id));
            pub.mech_name = thisMech?.fullName ?? "Usuario Eliminado";

            return pub;
        }
    }
}