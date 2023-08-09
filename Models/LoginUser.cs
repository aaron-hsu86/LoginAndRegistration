#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

public class LoginUser
{
    [Required]
    [Display(Name = "Email:")]
    public string EmailCheck {get; set;}
    [Required]
    [Display(Name = "Password:")]
    public string PasswordCheck {get; set;}
}