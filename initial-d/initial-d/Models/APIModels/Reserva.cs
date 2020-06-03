using System;

namespace initial_d.Models.APIModels
{
    public class Reserva
    {
        public string reserve_id { get; set; }
        public string serv_id { get; set; }
        public string appuser_id { get; set; }
        public DateTime? start_date_hour { get; set; }
        public DateTime? end_date_hour { get; set; }
        public string status_reserve_id { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? created_at { get; set; }
        public bool deleted { get; set; }

        public Reserva()
        {

        }
    }

    public class ReservaVM : Reserva
    {

        public Servicio serv { get; set; }
        public Usuario user { get; set; }

        public string userName {
            get
            {
                return user?.fullName ?? Resources.Messages.StringNotFound;
            }
        }
        public string servName
        {
            get
            {
                return serv?.name ?? Resources.Messages.StringNotFound;
            }
        }

        public string startDateTimeString
        {
            get
            {
                return start_date_hour?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            }
        }
        public string endDateTimeString
        {
            get
            {
                return end_date_hour?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            }
        }
        public string updatedAtString
        {
            get
            {
                return updated_at?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            }
        }
        public string createdAtString
        {
            get
            {
                return created_at?.ToString("dd/MM/yyyy HH:mm") ?? "-";
            }
        }

        public string statusString { get; set; }
    }
}