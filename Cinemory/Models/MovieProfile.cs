using System.ComponentModel.DataAnnotations.Schema;

namespace Cinemory.Models
{
    public class MovieProfile //extended information about the movie
    {
        public int Id { get; set; } //primary key

        public string? Synopsis { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }

        public int MovieId { get; set; } //foreign key, relates to Movie
        public Movie Movie { get; set; } //navigation, reference to Movie



        // Hüsom bu değeri controllerdan hesaplatcaz senlik bişi değil unutturma sileyim sonra bunu
        [NotMapped]
        public double? AverageRating { get; set; }
    }
}
