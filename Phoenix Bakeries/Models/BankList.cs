using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class BankList
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
    }

    public class GetBanks
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<BankData> data { get; set; }
    }

    public class BankData
    {
        public string name { get; set; }
        public string code { get; set; }
    }
}
