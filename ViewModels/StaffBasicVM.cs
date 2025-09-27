using System.ComponentModel.DataAnnotations;

namespace RetroTapes.ViewModels
{
    public class StaffBasicVm
    {
        public byte StaffId { get; set; }

        [Required] public string FirstName { get; set; } = "";
        [Required] public string LastName { get; set; } = "";


        [Required(ErrorMessage = "E-post krävs.")]
        [EmailAddress(ErrorMessage = "Ogiltig e-postadress.")]
        public string Email { get; set; } = "";


        [Required(ErrorMessage = "Telefonnummer krävs.")]
        public string PhoneNumber { get; set; } = "";
    }
}
