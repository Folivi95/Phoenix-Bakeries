using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class RecipientVM
    {
        public RecipientCodeData data { get; set; }
    }
    
    public class RecipientCodeData
    {
        public RecipientDataBank details { get; set; }
        public string recipient_code { get; set; }
    }

    public class RecipientDataBank
    {
        public string bank_name { get; set; }
    }
}
