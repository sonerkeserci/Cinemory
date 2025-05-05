namespace Cinemory.Models
{
    public class Rating
    {
        public int Id { get; set; } //primary key
        public int Score { get; set; } //a value from 1 to 10

        public int MovieId { get; set; } //foreign key, Movie related
        public Movie Movie { get; set; } //navigation

        public int UserId { get; set; } //foreign key, User related
        public User User { get; set; } //nav
    }
}
