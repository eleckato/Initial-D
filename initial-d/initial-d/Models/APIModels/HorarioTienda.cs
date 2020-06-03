using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class HorarioTienda
    {
        public string schedule_id { get; set; }
        public DateTime start_hour { get; set; }
        public DateTime start_lunch_hour { get; set; }
        public DateTime end_lunch_hour { get; set; }
        public DateTime end_hour { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }
        public HorarioTienda()
        {

        }
    }
}