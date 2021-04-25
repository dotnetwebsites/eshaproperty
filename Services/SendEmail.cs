using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EeshaProperty.Areas.Identity.Data;
using EeshaProperty.Data;
using Microsoft.AspNetCore.Identity;

namespace EeshaProperty.Services
{
    public class SendMail : IMailService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _repository;
        public SendMail(UserManager<ApplicationUser> userManager, ApplicationDbContext repository)
        {
            _userManager = userManager;
            _repository = repository;
        }

        public void SendEmail(string toEmail, string name, string subject, string content)
        {
            var toAddress = new MailAddress(toEmail, name);

            var smtp = new SmtpClient
            {
                Host = MailData.Host,
                Port = MailData.Port,
                EnableSsl = MailData.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = MailData.UseDefaultCredentials,
                Credentials = new NetworkCredential(MailData.FromAddress.Address, MailData.Password)
            };

            using (var message = new MailMessage(MailData.FromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

        public void SendEmail(Mail mailUserId, string toEmail, string name, string subject, string content)
        {
            var maildata = _repository.MailLibraries.Where(p => p.MailUserId == mailUserId.ToString()).FirstOrDefault();
            MailAddress fromAddress = new MailAddress(maildata.EmailAddress, maildata.EmailName);

            if (maildata != null)
            {
                var toAddress = new MailAddress(toEmail, name);

                var smtp = new SmtpClient
                {
                    Host = maildata.Host,
                    Port = maildata.Port,
                    EnableSsl = maildata.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = maildata.UseDefaultCredentials,
                    Credentials = new NetworkCredential(fromAddress.Address, maildata.Password)
                };

                Console.WriteLine("Sending an email message to {0} using the SMTP host {1}.", toAddress.Address, smtp.Host);

                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                })
                {
                    //smtp.Send(message);
                    //return "ok";
                    try
                    {
                        smtp.Send(message);
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                System.Threading.Thread.Sleep(5000);
                                smtp.Send(message);
                            }
                            else
                            {
                                Console.WriteLine("Failed to deliver message to {0}",
                                    ex.InnerExceptions[i].FailedRecipient);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in RetryIfBusy(): {0}",
                                ex.ToString());
                    }
                }
            }
        }

        public void SendEmail(string toEmail, string subject, string content)
        {
            var toAddress = new MailAddress(toEmail);

            var smtp = new SmtpClient
            {
                Host = MailData.Host,
                Port = MailData.Port,
                EnableSsl = MailData.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = MailData.UseDefaultCredentials,
                Credentials = new NetworkCredential(MailData.FromAddress.Address, MailData.Password)
            };

            using (var message = new MailMessage(MailData.FromAddress, toAddress)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }

    }

}
