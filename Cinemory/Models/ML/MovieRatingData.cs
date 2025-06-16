using Microsoft.ML.Data;

namespace Cinemory.Models.ML
{
    // ML.NET ile film öneri sistemi için veri modeli
    public class MovieRatingData
    {
        // Kullanıcı ID'si (string olarak tutulur)
        [LoadColumn(0)]
        public string UserId { get; set; }

        // Film ID'si
        [LoadColumn(1)]
        public int MovieId { get; set; }

        // Kullanıcının filme verdiği puan (1-10 arası)
        [LoadColumn(2)]
        public float Rating { get; set; }
    }

    // ML.NET tahmin çıktısı modeli
    public class MovieRatingPrediction
    {
        // Tahmin edilen puan/skor
        public float Score { get; set; }
    }
} 