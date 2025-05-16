using System.ComponentModel.DataAnnotations;

namespace SICProject.Models
{
    public class BookingmasterVM
    {
        [Key]
        public int BookingId { get; set; }
        [Display(Name = "Instrument Name")]
        [Required(ErrorMessage = "Please select instrument name")]
        public int? InstrumentId { get; set; }
        [Display(Name = "Student Name")]
        [Required(ErrorMessage = "Please select student name")]

        public int? StudentId { get; set; }
        [Display(Name = "Remarks")]
        [Required(ErrorMessage = "Please enter remarks")]

        public string? Remarks { get; set; }
        [Display(Name = "Booking Date")]

        public DateOnly? BookingDate { get; set; }
        [Display(Name = "Slot Start")]

        public TimeOnly? SlotStart { get; set; }
        [Display(Name = "Slot End")]

        public TimeOnly? SlotEnd { get; set; }
    }
}
