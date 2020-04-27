using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class ProdUnit
    {
        public string abbreviation { get; set; }
        public string name { get; set; }
        public string plural_name { get; set; }
        public DateTime create_at { get; set; }
        public DateTime update_at { get; set; }
        public bool deleted { get; set; }

        public ProdUnit()
        {

        }
    }
}