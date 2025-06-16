using Microsoft.ML;
using Microsoft.ML.Trainers;
using Cinemory.Models;
using Cinemory.Models.ML;
using Cinemory.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinemory.Services
{
    // ML.NET ile film öneri sistemi servisi
    public class MovieRecommendationService
    {
        private readonly CinemoryDbContext _context; // Veritabanı erişimi
        private readonly MLContext _mlContext; // ML.NET ana context
        private ITransformer _model; // Eğitilmiş model

        // Servis kurucu fonksiyonu
        public MovieRecommendationService(CinemoryDbContext context)
        {
            _context = context;
            _mlContext = new MLContext();
        }

        // Modeli eğiten fonksiyon
        public async Task TrainModel()
        {
            // Veritabanından tüm kullanıcı-film puanlamalarını çekiyoruz
            var ratings = await _context.Ratings
                .Select(r => new MovieRatingData
                {
                    UserId = r.UserId,
                    MovieId = r.MovieId,
                    Rating = r.Score
                })
                .ToListAsync();

            // ML.NET için veri setini oluşturuyoruz
            var trainingData = _mlContext.Data.LoadFromEnumerable(ratings);

            // Matrix Factorization algoritması ayarları
            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "UserId", // Kullanıcı sütunu
                MatrixRowIndexColumnName = "MovieId",   // Film sütunu
                LabelColumnName = "Rating",             // Puan sütunu
                NumberOfIterations = 20,                  // Eğitim iterasyon sayısı
                ApproximationRank = 100                   // Model karmaşıklığı
            };

            // Eğitim pipeline'ı oluşturuluyor
            var trainer = _mlContext.Recommendation().Trainers.MatrixFactorization(options);
            var trainingPipeline = _mlContext.Transforms.Conversion.MapValueToKey("UserId")
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("MovieId"))
                .Append(trainer);

            // Modeli eğitiyoruz
            _model = trainingPipeline.Fit(trainingData);
        }

        // Kullanıcıya önerilecek filmleri döndüren fonksiyon
        public async Task<List<Movie>> GetRecommendedMovies(string userId, int count = 5)
        {
            // Model yoksa eğit
            if (_model == null)
            {
                await TrainModel();
            }

            // Kullanıcının izlediği/puanladığı filmleri bul
            var userRatings = await _context.Ratings
                .Where(r => r.UserId == userId)
                .Select(r => r.MovieId)
                .ToListAsync();

            // Kullanıcının henüz izlemediği filmleri bul
            var unwatchedMovies = await _context.Movies
                .Where(m => !userRatings.Contains(m.Id))
                .ToListAsync();

            // Her film için tahmin yap ve skorunu al
            var predictions = new List<(Movie Movie, float Score)>();
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<MovieRatingData, MovieRatingPrediction>(_model);

            foreach (var movie in unwatchedMovies)
            {
                var prediction = predictionEngine.Predict(new MovieRatingData
                {
                    UserId = userId,
                    MovieId = movie.Id,
                    Rating = 0 // Tahmin için kullanılmaz
                });

                predictions.Add((movie, prediction.Score));
            }

            // En yüksek skorlu filmleri öneri olarak döndür
            return predictions
                .OrderByDescending(p => p.Score)
                .Take(count)
                .Select(p => p.Movie)
                .ToList();
        }
    }
} 