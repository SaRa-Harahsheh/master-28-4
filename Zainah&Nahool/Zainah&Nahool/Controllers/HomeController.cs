using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zainah_Nahool.Models;

namespace Zainah_Nahool.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _DB;

        public HomeController(MyDbContext db)
        {
            _DB = db;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            return View();
        }


        [HttpPost]
        public IActionResult contactUs(string Name, string Email, string Subject, string Message)
        {
           
            if (string.IsNullOrEmpty(Email))
            {
               
                ViewBag.Error = "البريد الإلكتروني مطلوب.";
                return View("ContactUs"); 
            }


            var contactMessage = new ContactU
            {

                Name = Name,
                Email = Email,
                Subject = Subject,
                Message = Message,
                CreatedAt = DateTime.Now
            };

            _DB.ContactUs.Add(contactMessage);
            _DB.SaveChanges();

            //////////////////////// I have a problem here it is not sending the email
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress("sara.harahsheh0@gmail.com", "موقع زينه ونحول");
                mail.To.Add("sara.harahsheh0@gmail.com");
                mail.Subject = $"{Subject}";
                mail.Body = $"الاسم: {Name}\nالبريد: {Email}\n\nالرسالة:\n{Message}";
                mail.ReplyToList.Add(new MailAddress(Email));

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("sara.harahsheh0@gmail.com", "ezpw wtpi ntzw hbjf"),
                    EnableSsl = true
                };

                smtp.Send(mail);
            }
            catch (SmtpException smtpEx)
            {
                ViewBag.Error = smtpEx.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("ContactUs");
        }



       





            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
