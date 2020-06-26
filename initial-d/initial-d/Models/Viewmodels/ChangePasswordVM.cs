using System;
using System.ComponentModel.DataAnnotations;

namespace initial_d.Models.Viewmodels
{
    public class ChangePasswordVM
    {
        [Required]
        [Display(Name = "Contraseña Actual")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string oldPassword1 { get; set; }

        [Required]
        [Display(Name = "Repita su Contraseña Antigua")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string oldPassword2 { get; set; }

        [Required]
        [Display(Name = "Nueva Contraseña")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string newPassword1 { get; set; }

        [Required]
        [Display(Name = "Repita su Nueva Contraseña")]
        [StringLength(25, ErrorMessage = "Debe tener menos de {1} caracteres")]
        public string newPassword2 { get; set; }

        public ChangePasswordVM()
        {
            oldPassword1 = string.Empty;
            oldPassword2 = string.Empty;
            newPassword1 = string.Empty;
            newPassword2 = string.Empty;
        }
    }
}