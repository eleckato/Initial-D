using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class BookingRestriction
    {
        public string restriction_id { get; set; }
        public string serv_id { get; set; }
        public DateTime? start_date_hour { get; set; }
        public DateTime? end_date_hour { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? created_at { get; set; }
        public bool deleted { get; set; }

        public BookingRestriction()
        {

        }
    }

    public class BookingRestVM : BookingRestriction
    {
        private readonly string dateFormat = "dd/MM/yyyy HH:mm";

        public Servicio serv { get; set; }

        public string servName { get { return serv?.name ?? Resources.Messages.StringNotFound; } }

        public string startDateTimeString { get { return start_date_hour?.ToString(dateFormat) ?? "-"; } }
        public string endDateTimeString { get { return end_date_hour?.ToString(dateFormat) ?? "-"; } }
        public string updatedAtString { get { return updated_at?.ToString(dateFormat) ?? "-"; } }
        public string createdAtString { get { return created_at?.ToString(dateFormat) ?? "-"; } }

        public BookingRestVM()
        {

        }
    }
}