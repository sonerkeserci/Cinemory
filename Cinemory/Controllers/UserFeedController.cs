using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cinemory.Models;
using Cinemory.Models.ViewModels;
using Cinemory.Data;


namespace Cinemory.Controllers
{
    public class UserFeedController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly CinemoryDbContext _context;

        public UserFeedController(UserManager<AppUser> userManager, CinemoryDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            //watclisted movies
            var watchlisted = await _context.MovieWatchlistConnections  // MovieWatchlistConnection tablosundaki tüm satırları al

             .Where(mw => mw.Watchlist != null && mw.Watchlist.UserId == user.Id) //  UserId'ye Watchlist üzerinden eriş, Watchlist'i boş olmayan ve o Watchlist'in UserId’si şu anki kullanıcıya ait olanları seç
             .Include(mw => mw.Movie)   // her MovieWatchlistConnection nesnesine bağlı Movie nesnesini veritabanından birlikte çek
                 .ThenInclude(m => m.Profile) // her Movie için ayrıca onun Profile’ını da yükle
             .Select(mw => mw.Movie.Profile!) // sadece MovieProfile nesnelerini al
             .ToListAsync(); // sorguyu veritabanına gönder ve sonucu listele


            // watched 
            var watched = await _context.Ratings
            
                .Where(r => r.UserId == user.Id && r.Score > 0) // kullanıcının verdiği puanlar
                .Include(r => r.Movie)       // Movie'yi dahil et
                    .ThenInclude(m => m.Profile)   // Movie'nin profilini de dahil et
                .Select(r => r.Movie.Profile!)     // sadece Profile nesnesini al
                .Distinct()            // aynı film birden fazla kez puanlandıysa tekrar etmesin diye şu ekleniyomuş
                .ToListAsync();





            return View();
        }
    }
}
