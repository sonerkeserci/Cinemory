namespace Cinemory.Models
{
    public class Director
    {
        public int Id { get; set; } //primary key
        public string? FullName { get; set; }

        public ICollection<Movie>? Movies { get; set; } //navigation, Movie one to many
    }
}
