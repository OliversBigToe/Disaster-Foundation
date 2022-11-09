using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APPR6312_Assignment.Models
{
    public class Disasters
    {
        [Key]
        [DisplayName("ID")]
        public int disasterID { get; set; }

        [Required]
        [DisplayName("Disaster Name")]
        public string disasterName { get; set; }

        [Required]
        [DisplayName("Start Date")]
        public DateTime disasterStartDate { get; set; }

        [Required]
        [DisplayName("End Date")]
        public DateTime disasterEndDate { get; set; }

        [Required]
        [DisplayName("Disaster Location")]
        public string disasterLocation { get; set; }

        [Required]
        [DisplayName("Aid Type")]
        public string aidType { get; set; }

        [Required]
        [DisplayName("Description")]
        public string disasterDescription { get; set; }

        [Required]
        [DisplayName("Allocated Money")]
        public decimal allocatedMoney { get; set; }

        [Required]
        [DisplayName("Allocated Goods")]
        public int allocatedGoods { get; set; }

        [DisplayName("Goods Category")]
        public string goodsCategory { get; set; }
    }
}
