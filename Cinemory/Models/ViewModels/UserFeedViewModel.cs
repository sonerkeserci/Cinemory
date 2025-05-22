namespace Cinemory.Models.ViewModels
{
    public class UserFeedViewModel
    {
        public string UserName { get; set; }

        // Kullanıcının izlemek istediği ama henüz izlemediği filmler
        public List<MovieProfile> WatchlistedMovies { get; set; }

        // Kullanıcının izlediği ve puanladığı filmler
        public List<MovieProfile> WatchedMovies { get; set; }

        // Sistem tarafından önerilen filmler (AI veya kurallı)
        public List<MovieProfile> RecommendedMovies { get; set; }

        

        // Kullanıcının ID'si (gerekirse)
        public string? UserId { get; set; }

        // Dış haber iframe için
        public string WidgetUrl { get; set; } = "https://www.tasteofcinema.com";
    }
}
