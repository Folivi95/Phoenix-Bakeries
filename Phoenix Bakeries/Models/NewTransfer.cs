using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix_Bakeries.Models
{
    public class NewTransfer
    {
        public int ID { get; set; }
        public string Nuban { get; set; }
        [StringLength(15)]
        public string Description { get; set; }
        [Required, StringLength(10)]
        public string AccountNumber { get; set; }
        [Display(Name = "Bank Name"), Required]
        public string BankCode { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
