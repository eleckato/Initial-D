using System;
using System.ComponentModel.DataAnnotations;

namespace initial_d.Models.APIModels
{
    public class Producto
    {
        public string product_id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string product_desc { get; set; }

        [Required]
        [Display(Name = "Stock")]
        public int stock { get; set; }

        [Required]
        [Display(Name = "Marca")]
        public string brand { get; set; }

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime create_at { get; set; }

        [Required]
        [Display(Name = "Ultima Actualización")]
        public DateTime update_at { get; set; }

        [Required]
        [Display(Name = "Eliminado")]
        public bool deleted { get; set; }


        [Required]
        [Display(Name = "Unidad")]
        public string unit_id { get; set; }
        public ProdUnit Unit { get; set; }


        [Required]
        [Display(Name = "Status")]
        public string product_status { get; set; }
        public string status_name { get; set; }

        public Producto()
        {

        }

        public Producto(bool isTemplate)
        {
            product_id = string.Empty;
            name = string.Empty;
            product_desc = string.Empty;
            stock = 0;
            brand = string.Empty;
            unit_id = string.Empty;
            product_status = string.Empty;
            create_at = DateTime.Now;
            update_at = DateTime.Now;
            deleted = false;

            status_name = string.Empty;
            Unit = new ProdUnit();
        }
    }
}