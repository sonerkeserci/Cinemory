using Microsoft.AspNetCore.Identity;

namespace Cinemory.Models
{
    public class AppUser : IdentityUser
    {
        
        public string FullName { get; set; }
    }
}
