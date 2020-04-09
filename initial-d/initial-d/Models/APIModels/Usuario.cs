using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace initial_d.Models.APIModels
{
    public class Usuario
    {
        public string appuser_id { get; set; }
        public string hash { get; set; }
        public bool isdelete { get; set; }


        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string username { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Nombres")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string last_names { get; set; }

        [Required]
        [Display(Name = "Dirección")]
        public string adress { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string phone { get; set; }

        [Required]
        [Display(Name = "Descripticón")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Ultimo Loguin")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime lastlogin { get; set; }

        [Required]
        [Display(Name = "Tipo")]
        public string user_type_id { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string status_id { get; set; }

        public Usuario()
        {

        }

        public Usuario(bool isTemplate)
        {
            appuser_id = string.Empty;
            username = string.Empty;
            email = string.Empty;
            name = string.Empty;
            last_names = string.Empty;
            adress = string.Empty;
            phone = string.Empty;
            description = string.Empty;
            user_type_id = string.Empty;

            hash = null;
            isdelete = false;
            lastlogin = DateTime.Today;
            status_id = "1";
        }
    }
}