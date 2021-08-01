using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class PaymentViewModel : _BaseViewModel
    {
        public Decimal Amount { get; set; }
    }
}