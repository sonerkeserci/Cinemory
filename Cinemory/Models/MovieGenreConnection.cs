namespace Cinemory.Models
{
    public class MovieGenreConnection //many to many relation between Movie and Genre
    {
        public int MovieId { get; set; } //foreign key, relates to Movie
        public Movie? Movie { get; set; } //navigation, reference to Movie

        public int GenreId { get; set; } //foreign key, relates to Genre
        public Genre? Genre { get; set; } //navigation, reference to Genre
    }
}
