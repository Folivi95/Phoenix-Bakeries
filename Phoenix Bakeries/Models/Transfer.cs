using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class Transfer
    {
        public int ID { get; set; }
        public string source { get; set; }
        [Display(Name = "Reason")]
        public string reason { get; set; }
        public int amount { get; set; }
        public string recipient { get; set; }

    }
}
