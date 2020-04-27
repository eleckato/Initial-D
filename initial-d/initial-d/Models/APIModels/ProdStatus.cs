using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class ProdStatus
    {
        public string status_id { get; set; }
        public string status { get; set; }
        public DateTime create_at { get; set; }
        public DateTime update_at { get; set; }
        public bool deleted { get; set; }

        public ProdStatus()
        {

        }
    }
}