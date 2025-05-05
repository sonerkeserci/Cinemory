namespace Cinemory.Models
{
    public class MovieWatchlistConnection //many to many relation between Movie and Watchlist
    {
        public int MovieId { get; set; } //foreign key
        public Movie Movie { get; set; } //nav

        public int WatchlistId { get; set; } //foreign key
        public Watchlist Watchlist { get; set; } //nav
    }
}
