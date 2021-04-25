using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;

namespace EeshaProperty.Models
{
    public class MailLibrary
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string MailUserId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string EmailAddress { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string EmailName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string Password { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }

        // [NotMapped]
        // public MailAddress FromAddress = new MailAddress(EmailAddress, EmailName);
    }
}