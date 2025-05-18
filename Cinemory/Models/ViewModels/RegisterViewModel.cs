using System.ComponentModel.DataAnnotations;

namespace Cinemory.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Username:")]
        [Required(ErrorMessage = "Please enter your user name.")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Email adress:")]
        [Required(ErrorMessage = "Please enter a valid email adress.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Display(Name = "Password:")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Confirm your password:")]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
