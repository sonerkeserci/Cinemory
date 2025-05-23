namespace Cinemory.Models
{
    public class Rating
    {
        public DateTime DateRated { get; set; } = DateTime.UtcNow;


        public int Id { get; set; } //primary key
        public int Score { get; set; } //a value from 1 to 10

        public int MovieId { get; set; } //foreign key, Movie related
        public Movie? Movie { get; set; } //navigation

        public required string UserId { get; set; } //foreign key, User related
        public AppUser? User { get; set; } //nav
    }
}
