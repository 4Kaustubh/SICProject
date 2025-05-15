using System.ComponentModel.DataAnnotations;

namespace SICProject.Models
{
    public class InstrumentsmasterVM
    {
        [Key]
        public int InstrumentsId { get; set; }
        [Display(Name = "Instrument Name")]
        [Required(ErrorMessage = "Instrument Name is required")]
        public string InstrumentName { get; set; } = null!;
        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Remarks are required")]
        public string Remarks { get; set; } = null!;
    }
}
