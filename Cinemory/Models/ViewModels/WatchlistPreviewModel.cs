using Cinemory.Models;
using Cinemory.Data;

namespace Cinemory.Models.ViewModels
    
{
    public class WatchlistPreviewModel
    {
        public string WatchlistName { get; set; } = string.Empty; //watchlist name, user's watchlist
        public int WatchlistId { get; set; } //primary key
        public int TotalMovies { get; set; } //watchlist movie count

        public string UserId { get; set; } = string.Empty; //UserId, AppUser tablosundaki Id ile eşleşir
        public string UserName { get; set; } = string.Empty; //UserName, AppUser tablosundaki UserName ile eşleşir

        public List<Movie> PreviewMovies { get; set; } = new List<Movie>(); //frist 3 movies in the watchlist for preview


    }
}
