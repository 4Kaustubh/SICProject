using System.ComponentModel.DataAnnotations;

namespace SICProject.Models
{
    public class HolidaymasterVM
    {
        [Key]
        public int HolidayId { get; set; }
        [Display(Name = "Holiday Date")]
        [Required(ErrorMessage = "Holiday Date is required")]

        public DateOnly? HolidayDate { get; set; }
        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Remarks are required")]
        public string? Reson { get; set; }
    }
}
