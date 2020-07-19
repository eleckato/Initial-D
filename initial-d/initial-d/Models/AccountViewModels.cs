using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace initial_d.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Mantener Sesión Iniciada")]
        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El campo es obligatorio")]
        [EmailAddress(ErrorMessage = "El mail ingresado no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
