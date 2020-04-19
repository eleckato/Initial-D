using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class PublicStatus
    {
        public string user_type_id { get; set; }
        public string status_name { get; set; }
        public DateTime create_at { get; set; }
        public DateTime update_at { get; set; }
        public bool deleted { get; set; }
    }
}