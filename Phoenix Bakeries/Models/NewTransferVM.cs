using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class NewTransferVM
    {
        public string status { get; set; }
        public string message { get; set; }
        public TransferVM data { get; set; }
    }

    public class TransferVM
    {
        public string transfer_code { get; set; }
    }
}
