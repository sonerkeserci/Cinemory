using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Cinemory.Models
{
    public class User
    {
        public int Id { get; set; }  //primary key
        public string Username { get; set; }

        public UserProfile Profile { get; set; } //navigation, UserProfile one-to-one connection
        public ICollection<Review> Reviews { get; set; } //navigation, Review one-to-many connection
        public ICollection<Rating> Ratings { get; set; } //navigation, Rating one-to-many connection
        public ICollection<Watchlist> Watchlists { get; set; } //navigation, Watchlist one-to-many connection
        public ICollection<FavoriteMovie> FavoriteMovies { get; set; }

    }
}
