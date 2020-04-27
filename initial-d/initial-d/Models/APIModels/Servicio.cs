using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class Servicio
    {
        public string serv_id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string serv_desc { get; set; }
        public int estimated_time { get; set; }
        public string serv_status { get; set; }
        public DateTime create_at { get; set; }
        public DateTime update_at { get; set; }
        public bool deleted { get; set; }

        public Servicio()
        {

        }

        public Servicio(bool isTemplate)
        {
            serv_id = string.Empty;
            name = string.Empty;
            price = 0;
            serv_desc = string.Empty;
            estimated_time = 0;
            serv_status = string.Empty;
            create_at = DateTime.Now;
            update_at = DateTime.Now;
            deleted = false;
        }
    }
}