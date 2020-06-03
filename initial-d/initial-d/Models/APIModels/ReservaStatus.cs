using System;

namespace initial_d.Models.APIModels
{
    public class ReservaStatus
    {
        public string status_reserve_id { get; set; }
        public string name { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public bool deleted { get; set; }

        public ReservaStatus()
        {

        }
    }
}