using Microsoft.VisualBasic;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace APPR6312_Assignment.Models
{
    public class Inventories
    {
        [Key]
        [DisplayName("ID")]
        public int invID { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime invDate { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Amount")]
        public int invAmount { get; set; }

        [DisplayName("Type")]
        public string invCategory { get; set; }
    }
}

