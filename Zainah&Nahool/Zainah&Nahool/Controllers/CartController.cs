using Microsoft.AspNetCore.Mvc;

namespace Zainah_Nahool.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Payment()
        {
            return View();
        }
    }
}
