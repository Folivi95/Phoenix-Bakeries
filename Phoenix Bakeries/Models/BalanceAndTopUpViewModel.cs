using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class BalanceAndTopUpViewModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<Balance> data { get; set; }
    }

    public class Balance
    {
        public string balance { get; set; }
    }
}
