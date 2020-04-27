using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class ServStatus
    {
        public string status_id { get; set; }
        public string status { get; set; }
        public DateTime create_at { get; set; }
        public DateTime update_at { get; set; }
        public bool deleted { get; set; }

        public ServStatus()
        {

        }

        public ServStatus(bool isTemplate)
        {
            status_id = string.Empty;
            status = string.Empty;
            create_at = DateTime.Now;
            update_at = DateTime.Now;
            deleted = false;
        }
    }
}