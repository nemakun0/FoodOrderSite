using System;
using System.Collections.Generic;
using FoodOrderSite.Models;

namespace FoodOrderSite.Models.ViewModels
{
    public class RestaurantReviewsViewModel
    {
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public List<ReviewDetailViewModel> Reviews { get; set; } = new List<ReviewDetailViewModel>();
        
        // Özet İstatistikler
        public double AverageTasteRating { get; set; }
        public double AverageServiceRating { get; set; }
        public double AverageDeliveryRating { get; set; }
        public double AverageOverallRating { get; set; }
        public int TotalReviews { get; set; }
        
        // Yıldız dağılımı
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
    }

    public class ReviewDetailViewModel
    {
        public int ReviewId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FormattedDate => CreatedAt.ToString("dd.MM.yyyy");
        
        public int TasteRating { get; set; }
        public int ServiceRating { get; set; }
        public int DeliveryRating { get; set; }
        public int OverallRating { get; set; }
        public string Comment { get; set; }
        
        // Kullanıcı bilgileri
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string OrderShortId { get; set; }
    }
} 