using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Zainah_Nahool.Models;

namespace Zainah_Nahool.Controllers
{
    public class UserController : Controller
    {

        private readonly MyDbContext _DB;

        public UserController (MyDbContext db)
        {
            _DB = db;
        }

        public IActionResult register()
        {
            return View();
        }



        [HttpPost]
        public IActionResult HandelRegister(UserRegister user, IFormFile Image)
        {
            // تحقق إذا الإيميل موجود مسبقاً
            var existingUser = _DB.Users.FirstOrDefault(u => u.Email == user.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "البريد الإلكتروني مستخدم من قبل. الرجاء استخدام بريد آخر.");
                return View("register");
            }

            // إذا كان هناك صورة تم تحميلها
            if (Image != null && Image.Length > 0)
            {
                // تحديد مسار مجلد التخزين
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // إنشاء المجلد إذا مش موجود
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // تجهيز اسم الملف
                string fileName = Path.GetFileName(Image.FileName);
                string filePath = Path.Combine(uploadPath, fileName);

                // حفظ الصورة
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }

                // حفظ اسم الصورة بقاعدة البيانات
                user.Image = fileName;
            }


            User u = new User()
            {
                FirstName = user.firstName,
                LastName = user.lastName,
               
                Image = user.Image,
                Email = user.Email,
                Password = user.Password,
                Phone = user.Phone,
                Address = user.Address,
                CreatedAt = DateTime.Now,
                Role = "Customer",
            };


            if (ModelState.IsValid)
            {
              
                _DB.Users.Add(u);
                _DB.SaveChanges();
                return RedirectToAction("login"); 
            }

            return View("register");

        }






        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HandelLogin(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _DB.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);
                if (existingUser != null)
                {
                    // حفظ بيانات المستخدم في الجلسة
                    HttpContext.Session.SetInt32("UserId", existingUser.Id); // تأكدي أن User.Id موجود
                    HttpContext.Session.SetString("UserName", existingUser.FirstName + " " + existingUser.LastName);

                    // هون الجديد: نقرأ الـ ReturnUrl إذا موجود
                    if (TempData["ReturnUrl"] != null)
                    {
                        string returnUrl = TempData["ReturnUrl"].ToString();
                        return Redirect(returnUrl); // رجعيه للرابط اللي كان بدو يروحله
                    }


                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                }
            }
            return View("login");
        }


        // profile 

        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login"); // إذا لم يكن هناك مستخدم مسجل دخول
            }

            var user = _DB.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login"); // إذا لم يتم العثور على المستخدم
            }

            return View(user); // تمرير المستخدم إلى الصفحة
        }


        public IActionResult EditProfile(IFormCollection form, IFormFile profileImage)
        {
            //int? userId = HttpContext.Session.GetInt32("UserId"); // جلب الـ UserId من الجلسة
            //var user = _DB.Users.FirstOrDefault(u => u.Id == userId); // استخدام الـ UserId للبحث عن المستخدم

            //return View(user);


            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var currentUser = _DB.Users.FirstOrDefault(u => u.Id == userId);
            if (currentUser != null)
            {
                currentUser.FirstName = form["FirstName"];
                currentUser.LastName = form["LastName"];
                currentUser.Phone = form["Phone"];
                currentUser.Address = form["Address"];

                // نفس كود الصورة...

                _DB.SaveChanges();
                TempData["SuccessMessage"] = "تم التعديل بنجاح!";
            }

            return RedirectToAction("Profile");
        }




        [HttpPost]
        public IActionResult EditProfile(User user, IFormFile profileImage)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var currentUser = _DB.Users.FirstOrDefault(u => u.Id == userId);

            if (currentUser != null)
            {
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Phone = user.Phone;
                currentUser.Address = user.Address;

                if (profileImage != null && profileImage.Length > 0)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!string.IsNullOrEmpty(currentUser.Image) && currentUser.Image != "default.png")
                    {
                        string oldImagePath = Path.Combine(uploads, currentUser.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                    string fullPath = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        profileImage.CopyTo(stream);
                    }

                    currentUser.Image = fileName;
                }

                _DB.SaveChanges();
                TempData["SuccessMessage"] = "تم تعديل الملف الشخصي بنجاح!";
            }

            return RedirectToAction("Profile");
        }



        [HttpPost]
        public IActionResult ResetPassword(IFormCollection form)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["PasswordMessage"] = "يجب تسجيل الدخول لتغيير كلمة المرور.";
                return RedirectToAction("Login");
            }

            string currentPassword = form["CurrentPassword"];
            string newPassword = form["NewPassword"];
            string confirmPassword = form["ConfirmPassword"];

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["PasswordMessage"] = "يرجى تعبئة جميع الحقول.";
                return RedirectToAction("Profile");
            }

            if (newPassword != confirmPassword)
            {
                TempData["PasswordMessage"] = "كلمة المرور الجديدة وتأكيدها غير متطابقين.";
                return RedirectToAction("Profile");
            }

            var user = _DB.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null || user.Password != currentPassword)
            {
                TempData["PasswordMessage"] = "كلمة المرور الحالية غير صحيحة.";
                return RedirectToAction("Profile");
            }

            user.Password = newPassword;
            _DB.SaveChanges();

            TempData["PasswordMessage"] = "تم تغيير كلمة المرور بنجاح.";
            return RedirectToAction("Profile");
        }



















        //public IActionResult profile()
        //{
        //    int? userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //    {
        //        return RedirectToAction("login");
        //    }

        //    var user = _DB.Users.FirstOrDefault(u => u.Id == userId);
        //    if (user == null)
        //    {
        //        return RedirectToAction("login");
        //    }

        //    // تحويل كائن User إلى UserRegister إذا كنتِ تستخدمينه في الصفحة
        //    User userReg = new User
        //    {
        //        FirstName = user.FirstName,
        //        LastName = user.LastName,
        //        Email = user.Email,
        //        Phone = user.Phone,
        //        Address = user.Address,
        //        Role = user.Role,
        //        Image = user.Image,
        //        CreatedAt = user.CreatedAt
        //    };

        //    return View(userReg); // تمرير UserRegister لعرضه في الـ View
        //}
    }
}
