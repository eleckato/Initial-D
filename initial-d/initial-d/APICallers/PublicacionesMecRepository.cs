using initial_d.Common;
using initial_d.Models.APIModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace initial_d.APICallers
{
    public class PublicacionesMecRepository : RepositoryBase
    {
        private readonly string prefix = "????";

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


        // TODO Connection with API
        // TODO Search filters
        // TODO Pagination
        /// <summary>
        /// API call to list all the Mechanic Publications
        /// </summary>
        public IEnumerable<PublicacionMec> GetAllPub()
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

                return mockData.Where(x => !x.deleted).ToList();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }


        // TODO Search filters
        // TODO Pagination
        /// <summary>
        /// API call to list all Deleted Publications
        /// </summary>
        public IEnumerable<PublicacionMec> GetAllDeletedPub()
        {
            try
            {
                return mockData.Where(x => x.deleted).ToList();
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }


        // TODO Connection with API
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
                //var request = new RestRequest($"{prefix}/mechanics/{mechId}", Method.GET)
                //{
                //    RequestFormat = DataFormat.Json
                //};

                //var response = client.Execute<Mecanico>(request);

                //string notFoundMsg = "El Mecánico requerido no existe";
                //CheckStatusCode(response, notFoundMsg);

                //return response.Data;

                return mockData.SingleOrDefault(x => x.public_id.Equals(pubId));
            }
            catch (Exception e)
            {
                ErrorWriter.ExceptionError(e);
                return null;
            }
        }


        // TODO Connection with API
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
                // API CALL

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


        // TODO Connection with API
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