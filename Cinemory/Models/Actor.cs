namespace Cinemory.Models
{
    public class Actor
    {
        public int Id { get; set; } //primary key
        public string FullName { get; set; }

        public ICollection<MovieActorConnection> Movies { get; set; } //navigation, many to many via MovieActorConnection class
    }
}
