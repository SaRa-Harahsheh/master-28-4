using Zainah_Nahool.Models;

namespace Zainah_Nahool.ViewModels
{
    public class ProductFeedbackViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public decimal Price { get; set; } // 👈 أضفنا خاصية Price

        public string? Image { get; set; }
        public int FeedbackCount { get; set; }

        public List<FeedbackItemViewModel> Feedbacks { get; set; } = new();
    }

    public class FeedbackItemViewModel
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string Comment { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime DatePosted { get; set; }
    }



}

