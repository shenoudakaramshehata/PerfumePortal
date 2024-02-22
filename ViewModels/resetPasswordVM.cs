using System.ComponentModel.DataAnnotations;

namespace CRM.ViewModels
{
	public class resetPasswordVM
    {
        [Required(ErrorMessage ="Enter your new password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage ="Confirm your new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage = "Confirm Password is not matched ")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Enter your old password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
    }
}
