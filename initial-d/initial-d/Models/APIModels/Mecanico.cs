﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class Mecanico : Usuario
    {

        [Display(Name = "Publicación Pendiente")]
        public bool hasPendingPublication { get; set; }

        [Display(Name = "Tiene Deuda")]
        public bool hasDebt { get; set; }

        public Mecanico()
        {

        }

        public Mecanico(bool isTemplate)
        {
            appuser_id = string.Empty;
            username = string.Empty;
            email = string.Empty;
            name = string.Empty;
            last_names = string.Empty;
            adress = string.Empty;
            phone = string.Empty;
            lastlogin = DateTime.Today;
            mail_confirmed = false;
            user_type_id = string.Empty;
            user_type_name = string.Empty;
            status_id = string.Empty;
            status_name = string.Empty;
            updated_at = DateTime.Today;
            created_at = DateTime.Today;
            deleted = false;
            hasPendingPublication = false;
            hasDebt = false;
        }

    }
}