using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.MVC.Controllers;
using UserManagement.MVC.Data;
using UserManagement.MVC.Models;

namespace UserManagement.MVC
{
    public class AutoEmailSender
    {
        public AutoEmailSender(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            this.webHostingEnvironment = webHostingEnvironment;
            this.configuration = configuration;
        }

        public void AutoSendEmail()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"));

            using (var _context = new ApplicationDbContext(optionsBuilder.Options))
            {
                var dateAndTime = DateTime.Now;
                var dateToday = dateAndTime.Date;

                var data = (from bookReq in _context.BookRequests
                            join user in _context.Users on bookReq.BookRequestedBy equals user.Id into bookRequser
                            from user in bookRequser.DefaultIfEmpty()
                            select new BookRequestViewModel
                            {

                                BookRequestId = bookReq.BookRequestId,
                                BookRequestedBy = bookReq.BookRequestedBy,
                                Fullname = $"{user.FirstName} {user.LastName}",
                                FromDate = bookReq.FromDate,
                                ToDate = bookReq.ToDate,
                                BookId = bookReq.BookId,
                                BookName = bookReq.BookInventory.BookName
                            }).ToList();

                foreach (var item in data)
                {
                   if(item.ToDate < dateToday)
                   {
                        var userEmail = (from bookReq in _context.BookRequests
                                         join user in _context.Users on bookReq.BookRequestedBy equals user.Id into bookRequser
                                         from user in bookRequser.DefaultIfEmpty()
                                         where bookReq.BookRequestId == item.BookRequestId
                                         select user.Email).FirstOrDefault();

                        var api = new EmailController(this.webHostingEnvironment, this.configuration);
                        api.SendEmail(userEmail, "System", "Expire");
                    }

                    var bookRequest =  _context.BookRequests.Find(item.BookRequestId);
                    bookRequest.Status = Enums.Status.Expire.ToString();
                    _context.Update(bookRequest);
                    _context.SaveChanges();
                }
            }
        }

        private List<Timer> timers = new List<Timer>();
        private readonly IWebHostEnvironment webHostingEnvironment;
        private readonly IConfiguration configuration;

        public void ScheduleTask(int hour, int min, double intervalInHour, Action task)
        {
            DateTime now = DateTime.Now;
            DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);
            if (now > firstRun)
            {
                firstRun = firstRun.AddDays(1);
            }
            TimeSpan timeToGo = firstRun - now;
            if (timeToGo <= TimeSpan.Zero)
            {
                timeToGo = TimeSpan.Zero;
            }
            var timer = new Timer(x =>
            {
                task.Invoke();
            }, null, timeToGo, TimeSpan.FromHours(intervalInHour));
            timers.Add(timer);
        }

        public void IntervalInDays(int hour, int min, double interval, Action task)
        {
            interval = interval * 24;
            ScheduleTask(hour, min, interval, task);
        }

        //This is used for testing only
        public void IntervalInSeconds(int hour, int sec, double interval, Action task)
        {
            interval = interval / 3600;
            ScheduleTask(hour, sec, interval, task);
        }
    }
}
