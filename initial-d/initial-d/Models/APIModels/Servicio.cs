﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace initial_d.Models.APIModels
{
    public class Servicio
    {
        [Required]
        [Display(Name = "Id")]
        public string serv_id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        [StringLength(30, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public int price { get; set; }

        [Required]
        [Display(Name = "Precio")]
        public string priceString {
            get
            {
                return price.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        [Required]
        [Display(Name = "Descripción")]
        [StringLength(250, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string serv_desc { get; set; }

        [Required]
        [Display(Name = "Tiempo")]
        [Range(1, 9999, ErrorMessage = "Debe tener menos de 4 dígitos y ser mayor a 0")]
        public int estimated_time { get; set; }

        [Required]
        [Display(Name = "Tiempo")]
        public string timeString { 
            get
            {
                int time = estimated_time;
                string str;
                if (time < 60) str = $"{time} Minuto{(time==1?"":"s")}";
                else
                {
                    int h = (int)Math.Floor((double)time / 60);
                    int m = time - (h*60);
                    str = $"{h} Hora{(h==1?"":"s")}";
                    if (m > 0) str += $" y {m} Minuto{(m==1?"":"s")}";
                }

                return str;
            }
        }

        [Required]
        [Display(Name = "Status")]
        public string serv_status { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string status_name { get; set; }


        [Required]
        [Display(Name = "Es Periódico")]
        public bool is_recurring { get; set; }


        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime created_at { get; set; }

        [Required]
        [Display(Name = "Última actualización")]
        public DateTime updated_at { get; set; }

        [Required]
        [Display(Name = "Eliminado")]
        public bool deleted { get; set; }

        public Servicio()
        {
            is_recurring = true;
        }

        public Servicio(bool isTemplate)
        {
            serv_id = string.Empty;
            name = string.Empty;
            serv_desc = string.Empty;

            price = 0;
            estimated_time = 0;
            is_recurring = false;

            serv_status = string.Empty;
            status_name = string.Empty;

            created_at = DateTime.Now;
            updated_at = DateTime.Now;
            deleted = false;
        }
    }
}