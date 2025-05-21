namespace Cinemory.Models.ViewModels
{
    public class MovieInteractionViewModel
    {
        public int MovieId { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsInWatchlist { get; set; }
        public int? Rating { get; set; }

        public string? Review { get; set; }
    }
}
