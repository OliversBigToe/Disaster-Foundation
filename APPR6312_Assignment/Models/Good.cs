using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APPR6312_Assignment.Models
{
    public class Good
    {
        [Key]
        [DisplayName("ID")]
        public int goodsID { get; set; }

        [Required]
        [DisplayName("Date")]
        public DateTime goodsDate { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Amount of Goods")]
        public int goodsAmount { get; set; }

        [Required]
        [DisplayName("Description")]
        public string goodsDescription { get; set; }

        [Required]
        [DisplayName("Category")]
        public string goodsCategory { get; set; }


        [DisplayName("Name")]
        public string goodsDonor { get; set; }
    }
}
