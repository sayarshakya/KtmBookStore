using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using UserManagement.MVC.Data;

namespace UserManagement.MVC.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [AllowAnonymous]
    public class EmailController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public EmailController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }
        public bool SendEmail(string ToId, string FromId, string Subject)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

            using (var _context = new ApplicationDbContext(optionsBuilder.Options))
            {
                try
                {
                    // create email message
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse("vesuviolabs2020@gmail.com"));
                    email.To.Add(MailboxAddress.Parse(ToId));
                    email.Subject = Subject;
                    if(Subject == Enums.Status.Approve.ToString())
                    {
                        email.Body = new TextPart(TextFormat.Plain) { Text = "Your book request has been approved by " + FromId + "." };
                    }
                    else if(Subject == Enums.Status.Denied.ToString())
                    {
                        email.Body = new TextPart(TextFormat.Plain) { Text = "Your book request has been denied by " + FromId + "." };
                    }
                    else
                    {
                        email.Body = new TextPart(TextFormat.Plain) { Text = "Your book issued date has been expired. Please return the book as soon as possible." };
                    }
                    

                    // send email
                    using var smtp = new SmtpClient();
                    smtp.Connect("smtp.gmail.com", 587, false);
                    smtp.Authenticate("vesuviolabs2020@gmail.com", "Home@123");
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        
    }
}