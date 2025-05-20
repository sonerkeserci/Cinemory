using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinemory.Models.ViewModels
{
    public class MovieEditViewModel
    {
        public int Id { get; set; } // Movie Id
        public string Name { get; set; } = null!;
        public int Year { get; set; }
        public int DirectorId { get; set; }

        // Many-to-many selections
        public List<int> SelectedGenreIds { get; set; } = new();
        public List<int> SelectedActorIds { get; set; } = new();

        // MovieProfile fields
        public string? Synopsis { get; set; }
        public string? PosterUrl { get; set; }
        public string? TrailerUrl { get; set; }

        // Dropdown lists
        public List<SelectListItem>? Directors { get; set; }
        public List<SelectListItem>? Genres { get; set; }
        public List<SelectListItem>? Actors { get; set; }
    }
}
