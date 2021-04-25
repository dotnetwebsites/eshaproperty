using System.ComponentModel.DataAnnotations;

namespace EeshaProperty.Models
{
    public class AccountModel
    {

    }

    public class ChangeEmailModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
    public class SendEmailConfirmLinkViewModel
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "Space not allowed, please enter valid username or email")]
        [Required(ErrorMessage = "Please enter username or email address.")]
        [Display(Name = "Email Address or Username")]
        public string Value { get; set; }

        public bool IsSuccess { get; set; }
    }
}