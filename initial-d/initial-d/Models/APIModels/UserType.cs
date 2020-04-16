using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class UserType
    {
        [Key]
        public string user_type_id { get; set; }
        public string name { get; set; }

        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }

        public UserType()
        {

        }
    }
}