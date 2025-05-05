namespace Cinemory.Models
{
    public class FavoriteMovie //a many-to-many relation between User and Movie
    {
        public int UserId { get; set; } //foreign key, User related
        public User User { get; set; } //navigation, User reference

        public int MovieId { get; set; } //foreign key, Movie related
        public Movie Movie { get; set; } //navigation, Movie reference

    }
}
