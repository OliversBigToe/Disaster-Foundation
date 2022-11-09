using Microsoft.VisualBasic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace APPR6312_Assignment.Models
{
    public class Cash
    {
        [Key]
        [DisplayName("ID")]
        public int moneyID { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime moneyDate { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Amount")]
        public decimal moneyAmount { get; set; }

        [DisplayName("Name")]
        public string goodsDonor { get; set; }
    }
}
