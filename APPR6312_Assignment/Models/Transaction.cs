using Microsoft.VisualBasic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace APPR6312_Assignment.Models
{
    public class Transaction
    {
        [Key]
        [DisplayName("ID")]
        public int transID { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime transDate { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Amount")]
        public decimal transAmount { get; set; }

        [DisplayName("Type")]
        public string transType { get; set; }
    }
}
