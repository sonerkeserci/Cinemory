namespace Cinemory.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty; // UserId, AppUser tablosundaki Id ile eşleşir
        public string UserName { get; set; } = string.Empty;        
        public DateTime JoinDate { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? PosterUrl { get; set; }  


        public int TotalMoviesWatched { get; set; }
        public int TotalReviewsWritten { get; set; }

        public string? Bio { get; set; }  

        public ICollection<MovieWatchlistConnection> UserWatchlist { get; set; }
        public ICollection<FavoriteMovie> FavoriteMovies { get; set; }
        public ICollection<Rating> UserRatings { get; set; }
        public ICollection<Review> UserReviews { get; set; }


    }
}
