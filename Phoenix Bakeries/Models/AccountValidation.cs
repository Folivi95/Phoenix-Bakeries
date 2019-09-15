using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class AccountValidation
    {
        public bool status { get; set; }
        public string message { get; set; }
        public AccDetails data { get; set; }
    }

    public class AccDetails
    {
        public string account_number { get; set; }
        public string account_name { get; set; }
    }
}
