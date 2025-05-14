using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SICProject.Models
{
    public class loginVM
    {
        [Key]
        [DisplayName("Email Address")]
        [RegularExpression("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please correct E-mail address")]
        [Required(ErrorMessage = "Please enter E-mail Id.")]
        public string? Email { get; set; }
        [DisplayName("Password")]
        [Required(ErrorMessage = "Please Enter Password")]
        public string? AppPwd { get; set; }
        public bool RememberMe { get; set; }
        [DisplayName("Mobile Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
        [Required(ErrorMessage = "Please Enter Mobile Number")]
        public string? AppMobileNumber { get; set; }

        [DisplayName("New Password")]
        [Required(ErrorMessage = "Please Enter New Password")]
        public string? AppPwdNew { get; set; }

    }
}
