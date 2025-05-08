using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Cinemory.Models
{
    public class UserProfile
    {
        public int Id { get; set; } //primary key

        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime JoinDate { get; set; }

        public int UserId { get; set; } //foreign key, User related, burada property ismini UserId yazdığımız için Entity Framework otomatik anlıyor bunun foreign key olduğunu abi japonlar yapmış
        public User? User { get; set; }  //navigation, reference to User, one to one relation

        //


    }
}
