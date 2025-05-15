using Cinemory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace Cinemory.Data
{
    public class CinemoryDbContext : IdentityDbContext<AppUser> //IdentityDbContext is used for authentication and authorization
    {
        public CinemoryDbContext(DbContextOptions<CinemoryDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<MovieActorConnection> MovieActorConnections { get; set; }
        public DbSet<MovieGenreConnection> MovieGenreConnections { get; set; }
        public DbSet<MovieWatchlistConnection> MovieWatchlistConnections { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<FavoriteMovie> FavoriteMovies { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<MovieProfile> MovieProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FavoriteMovie>()
                .HasKey(fm => new { fm.UserId, fm.MovieId });

            modelBuilder.Entity<MovieActorConnection>()
                .HasKey(mac => new { mac.MovieId, mac.ActorId });

            modelBuilder.Entity<MovieGenreConnection>()
                .HasKey(mgc => new { mgc.MovieId, mgc.GenreId });

            modelBuilder.Entity<MovieWatchlistConnection>()
                .HasKey(mwc => new { mwc.MovieId, mwc.WatchlistId });
        }



    }
}
