namespace Cinemory.Models.ViewModels
{
    public class UserFeedViewModel
    {
        public List<MovieInteractionViewModel> LastWatched { get; set; } = new();
        public List<MovieInteractionViewModel> Watchlisted { get; set; } = new();
        public List<MovieInteractionViewModel> RecentlyAdded { get; set; } = new();
        public List<MovieInteractionViewModel> Recommended { get; set; } = new();
        public string? RssWidgetUrl { get; set; }
    }
}
