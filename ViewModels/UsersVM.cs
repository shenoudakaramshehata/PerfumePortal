using System.ComponentModel.DataAnnotations;

namespace CRM.ViewModels
{
    public class UsersVM
    {
        public string? FullName { get; set; }
        public string? Mobile { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? CurrentEmail { get; set; }
       
        
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }
       
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$",ErrorMessage = "Should have at least one lower case , one upper case , one number,one special character and minimum length 6 characters")]
        [Compare("NewPassword", ErrorMessage = "The New Password and Confirmation Password do not match")]
        public string? ConfirmPassword { get; set; }
        public bool IsActive { get; set; }
        public string Id { get; set; }
    }
}
