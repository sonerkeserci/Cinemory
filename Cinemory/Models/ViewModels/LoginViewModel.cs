using System.ComponentModel.DataAnnotations;

namespace Cinemory.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Username:")]
        [Required(ErrorMessage = "Please enter your user name.")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Password:")]
        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
