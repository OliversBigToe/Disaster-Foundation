using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace APPR6312_Assignment.Models
{
    public class User
    {
        [Key]
        [DisplayName("Email")]
        [EmailAddress]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        public string userEmail { get; set; }

        [Required]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,50}$")]
        [StringLength(50, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string userPassword { get; set; }

        [Required]
        [DisplayName("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string userName { get; set; }

        [Required]
        [DisplayName("Surname")]
        [StringLength(50, MinimumLength = 3)]
        public string userSurname { get; set; }

        [Required]
        [DisplayName("Role")]
        public string userRole { get; set; } = "Pending";
    }
}
