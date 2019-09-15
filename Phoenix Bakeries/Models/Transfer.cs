using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class Transfer
    {
        public int ID { get; set; }
        public string source { get; set; }
        public string reason { get; set; }
        public int amount { get; set; }
        public string recipient { get; set; }

    }
}
