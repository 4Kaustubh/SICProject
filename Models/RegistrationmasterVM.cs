using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SICProject.Models
{
	public class RegistrationmasterVM
	{
		[Key]
		public long RegistrationId { get; set; }
		[Display(Name = "Student Name")]
		[Required(ErrorMessage = "Please enter student name")]
		public string? StudentName { get; set; }
		[Required(ErrorMessage = "Please select department name")]
		public int? DepartmentId { get; set; }
		[Display(Name = "Department Name")]
		public string? DepartmentName { get; set; }
		[Display(Name = "E-mail ID")]
		[RegularExpression("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please correct E-mail address")]
		[Required(ErrorMessage = "Please enter E-mail Id.")]
		public string? Email { get; set; }
		[DisplayName("Confirm E-mail ID")]
		[RegularExpression("^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$", ErrorMessage = "Please correct E-mail address")]
		[Required(ErrorMessage = "Please Re-enter E-mail Id.")]
		[Compare("Email", ErrorMessage = "E-mail Id not Matched!")]
		public string? ConfirmAppEmailId { get; set; }
		public bool? IsApproved { get; set; }

		public string? Remarks { get; set; }
		[Display(Name = "Mobile Number")]
		[RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be 10 digits.")]
		[Required(ErrorMessage = "Please enter mobile number")]
		public string? MobileNumber { get; set; }
		public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
