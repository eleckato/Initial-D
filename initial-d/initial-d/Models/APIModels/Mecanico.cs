using System;
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

    }
}