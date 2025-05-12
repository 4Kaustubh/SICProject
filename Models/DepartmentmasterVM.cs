using System.ComponentModel.DataAnnotations;

namespace SICProject.Models
{
    public class DepartmentmasterVM
    {
        [Key]
        public int DepartmentId { get; set; }
        [Display(Name = "Department Name")]
        [Required(ErrorMessage = "Please enter department name")]
        public string? DepartmentName { get; set; }
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }
}
