using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zainah_Nahool.Models;

public partial class UserRegister
{
    public int Id { get; set; }

    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Role { get; set; }

    public string? Image { get; set; }

    public DateTime? CreatedAt { get; set; }

   
}
