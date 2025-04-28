using Zainah_Nahool.Models;

namespace Zainah_Nahool.ViewModels
{
    public class AddFeedbackViewModel
    {
        public Feedback Feedback { get; set; } = new Feedback();
        public List<Product> Products { get; set; } = new List<Product>();
    }


}
