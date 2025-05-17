using System.ComponentModel.DataAnnotations;

namespace Cinemory.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your user name.")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter a valid email adress.")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;


        [Compare("Password",ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
