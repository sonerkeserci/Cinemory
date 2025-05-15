namespace Cinemory.Models
{
    public class FavoriteMovie //a many-to-many relation between AppUser and Movie
    {
        public required string UserId { get; set; } //foreign key, User related
        public AppUser? User { get; set; } //navigation, User reference

        public int MovieId { get; set; } //foreign key, Movie related
        public Movie? Movie { get; set; } //navigation, Movie reference

    }
}
