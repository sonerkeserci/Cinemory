namespace Cinemory.Models.ViewModels
{
    public class UserFeedViewModel
    {
        public required string UserName { get; set; } = "";
        public string? UserId { get; set; }

        // Kullanıcının izlemek istediği ama henüz izlemediği filmler
        public List<MovieProfile>? WatchlistedMovies { get; set; } = new List<MovieProfile>();

        // Kullanıcının izlediği ve puanladığı filmler
        public List<MovieProfile>? WatchedMovies { get; set; } = new List<MovieProfile>();

        // Sistem tarafından önerilen filmler (AI veya kurallı)
        public List<MovieProfile>? RecommendedMovies { get; set; } = new List<MovieProfile>();

        

        // Dış haber iframe için
        public string WidgetUrl { get; set; } = "https://www.tasteofcinema.com/category/features/";
    }
}
