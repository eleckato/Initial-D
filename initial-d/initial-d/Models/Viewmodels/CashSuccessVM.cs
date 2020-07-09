using initial_d.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Models.Viewmodels
{
    public class CashSuccessVM
    {
        public List<Servicio> recServList { get; set; }

        public string saleId { get; set; }

        public List<BookingVM> bookList { get; set; }
        public List<BookingRestVM> restList { get; set; }

        public List<BookingVM> templateBookList { get; set; }

        public CashSuccessVM()
        {

        }
    }
}