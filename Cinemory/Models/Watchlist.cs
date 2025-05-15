namespace Cinemory.Models
{
    public class Watchlist
    {
        public int Id { get; set; } //primary key
        public string? Name { get; set; }

        public required string UserId { get; set; } //foreign key
        public AppUser? User { get; set; }

        public ICollection<MovieWatchlistConnection>? Movies { get; set; } //navigation, many to many via MovieWatchlistConnection class
    }
}
