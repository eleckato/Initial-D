using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using initial_d.Models.APIModels;

namespace initial_d.Models.Viewmodels
{
    public class PointOfSaleVM
    {
        public string id { get; set; }

        public List<Prestacion> soldItems { get; set; }

        public string venId { get; set; }

        public Usuario user { get; set; }


        [Display(Name = "Total")]
        public int total { 
            get
            {
                var sumTotal = 0;
                soldItems.ForEach(x => sumTotal += x.total);

                return sumTotal;
            }
        }

        [Display(Name = "Total")]
        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public List<Producto> prodList { get; set; }
        public List<Servicio> servList { get; set; }
        public List<Usuario> userList { get; set; }

        public PointOfSaleVM()
        {
            id = Guid.NewGuid().ToString().Replace("-", "");
            soldItems = new List<Prestacion>();
            user = null;
            venId = string.Empty;
            prodList = new List<Producto>();
            servList = new List<Servicio>();
            userList = new List<Usuario>();
        }
    }

    public class Prestacion
    {
        [Required]
        [Display(Name = "Id")]
        public string id
        {
            get
            {
                if (!checkIntegrity()) return string.Empty;
                return prod?.product_id ?? serv.serv_id;
            }
        }

        [Required]
        [Display(Name = "Nombre")]
        public string name
        {
            get
            {
                if (!checkIntegrity()) return string.Empty;
                return prod?.name ?? serv.name;
            }
        }

        [Required]
        [Display(Name = "Descripción")]
        public string desc
        {
            get
            {
                if (!checkIntegrity()) return string.Empty;
                return prod?.product_desc ?? serv.serv_desc;
            }
        }

        [Required]
        [Display(Name = "Precio")]
        public int price
        {
            get
            {
                if (!checkIntegrity()) return 0;
                return prod?.price ?? serv.price;
            }
        }


        [Display(Name = "Precio")]
        public string priceString
        {
            get
            {
                return price.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        [Required]
        [Display(Name = "Total")]
        public int total
        {
            get
            {
                if (!checkIntegrity()) return 0;
                return amount * price;
            }
        }

        [Display(Name = "Total")]
        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }


        [Display(Name = "Tipo")]
        public string type {
            get
            {
                if (!checkIntegrity()) return string.Empty;
                return (prod != null) ? "Producto" : "Servicio";
            }
        }


        [Required]
        [Display(Name = "Cantidad")]
        public int amount { get; set; }

        public string amountString
        {
            get
            {
                if (!checkIntegrity()) return string.Empty;
                if (prod != null)
                {
                    string unitName = prod.Unit?.name ?? "";
                    string unitPruralName = prod.Unit?.plural_name ?? "";

                    string str = $"{amount} {(amount == 1 ? unitName : unitPruralName)}";

                    return str;
                }

                return "-";
            }
        }



        [Display(Name = "Producto")]
        public Producto prod { get; set; }

        [Display(Name = "Servicio")]
        public Servicio serv { get; set; }


        public Prestacion()
        {
            amount = 0;

            prod = null;
            serv = null;
        }

        private bool checkIntegrity()
        {
            if (prod == null && serv == null) return false;
            if (prod != null && serv != null) return false;
            return true;
        }
    }
}