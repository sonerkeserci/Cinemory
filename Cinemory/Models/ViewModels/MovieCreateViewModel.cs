using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinemory.Models.ViewModels
{
    public class MovieCreateViewModel
    {
        public string Name { get; set; }=null!;
        public int Year { get; set; }
        public int DirectorId { get; set; }

        public List<SelectListItem>? Directors { get; set; }
        public List<SelectListItem>? Genres { get; set; }
        public List<SelectListItem>? Actors { get; set; }

        public List<int> SelectedGenreIds { get; set; } = new();
        public List<int> SelectedActorIds { get; set; } = new();
    }
}
