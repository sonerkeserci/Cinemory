namespace Cinemory.Models
{
    public class Genre
    {

        public int Id { get; set; } //primary key
        public string Name { get; set; }

        public ICollection<MovieGenreConnection> Movies { get; set; } //navigation, many-to-many with Movie via MovieGenreConnection class
    }
}
