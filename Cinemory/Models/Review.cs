namespace Cinemory.Models
{
    public class Review
    {
        public int Id { get; set; } //primary key
        public string? Entry { get; set; }
        public DateTime CreatedAt { get; set; }

        public int MovieId { get; set; } //foreign key, Movie related
        public Movie? Movie { get; set; } //navigation, Movie reference

        public required string UserId { get; set; } //foreign key, User related
        public AppUser? User { get; set; } //navigation, User reference
    }
}
