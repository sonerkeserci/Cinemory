namespace Cinemory.Models
{
    public class MovieActorConnection //many to many relation between Movie and Actor classes
    {
        public int MovieId { get; set; } //foreign key
        public Movie Movie { get; set; } //nav

        public int ActorId { get; set; } //foreign key
        public Actor Actor { get; set; } //nav
    }
}
