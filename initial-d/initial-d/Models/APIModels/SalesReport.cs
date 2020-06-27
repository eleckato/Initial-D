using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace initial_d.Models.APIModels
{
    public class SalesReport
    {
        public List<SRSale> sale_list { get; set; }

        public List<SRProd> prod_sold_list { get; set; }

        public List<SRServ> serv_sold_list { get; set; }

        [Required]
        [Display(Name = "Desde")]
        public DateTime? date_start { get; set; }
        [Required]
        [Display(Name = "Hasta")]
        public DateTime? date_end { get; set; }

        [Required]
        [Display(Name = "Total")]
        public int total { get; set; }


        [Display(Name = "Fecha de la Venta")]
        public string Range
        {
            get
            {
                string startString = date_start?.ToString("dd/MM/yyyy") ?? "-";
                string endsTRING = date_end?.ToString("dd/MM/yyyy") ?? "-";

                return $"Desde {startString} hasta {endsTRING}";
            }
        }

        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public string prodTotalString
        {
            get
            {
                int t = 0;
                prod_sold_list.ForEach(x => { t += x.prod_total; });

                return t.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public string servTotalString
        {
            get
            {
                int t = 0;
                serv_sold_list.ForEach(x => { t += x.serv_total; });

                return t.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public SalesReport()
        {

        }
    }

    public class SRSale : Sale
    {
        [Required]
        [Display(Name = "Vendedor/a")]
        public Usuario seller { get; set; }
        [Required]
        [Display(Name = "Cajero/a")]
        public Usuario cashier { get; set; }
        [Required]
        [Display(Name = "Usuario/a")]
        public Usuario user { get; set; }
        [Required]
        [Display(Name = "Status")]
        public SaleStatus sale_status { get; set; }


        public string totalString
        {
            get
            {
                return total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public SRSale()
        {

        }
    }

    public class SRProd : Producto
    {
        [Required]
        [Display(Name = "Cantidad")]
        public int prod_n { get; set; }


        [Required]
        [Display(Name = "Total")]
        public int prod_total { get; set; }

        
        public string prodTotalString
        {
            get
            {
                return prod_total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }


        public string prodNString
        {
            get
            {
                string unitName = Unit?.name ?? "";
                string unitPruralName = Unit?.plural_name ?? "";

                string str = $"{prod_n} {(prod_n == 1 ? unitName : unitPruralName)}";
                return str;
            }
        }


        public SRProd()
        {

        }
    }

    public class SRServ : Servicio
    {
        [Required]
        [Display(Name = "Cantidad")]
        public int serv_n { get; set; }
        [Required]
        [Display(Name = "Total")]
        public int serv_total { get; set; }

        public string servTotalString
        {
            get
            {
                return serv_total.ToString("C", CultureInfo.CreateSpecificCulture("es-CL"));
            }
        }

        public SRServ()
        {

        }
    }

}