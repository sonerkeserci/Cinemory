using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.IO;

namespace Cinemory.Models
{
    public class Movie
    {
        public int Id { get; set; }  //primary key
        public string? Name { get; set; }
        public int Year { get; set; }

        public int DirectorId { get; set; }  //foreign key: relates to Director
        public Director? Director { get; set; } //navigation: reference to Director


        public MovieProfile? Profile { get; set; } //navigation, MovieProfile one-to-one 
        public ICollection<MovieGenreConnection>? Genres { get; set; } //navigation,MovieGenreConnection one-to-many 
        public ICollection<Review>? Reviews { get; set; } //navigation, Review one-to-many
        public ICollection<Rating>? Ratings { get; set; } //navigation, Rating one-to-many

        public ICollection<FavoriteMovie>? FavoritedByUsers { get; set; }

    }
}
