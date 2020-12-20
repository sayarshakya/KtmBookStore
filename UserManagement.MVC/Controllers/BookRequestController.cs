using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserManagement.MVC.Data;
using UserManagement.MVC.Models;

namespace UserManagement.MVC.Controllers
{
    public class BookRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public BookRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        // GET: BookRequest
        public async Task<IActionResult> Index()
        {
            return View(await _context.BookInventories.ToListAsync());
        }

        // GET: BookRequest/Create
        public async Task<IActionResult> CreateRequest(int? id)
        {
            var book = await _context.BookInventories.FindAsync(id);
            ViewBag.BookName = book.BookName;
            var model = new BookRequest();
            model.BookId = id ?? 0;
            model.FromDate = DateTime.Now;
            model.ToDate = DateTime.Now;
            return View(model);
        }

        // POST: BookRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(BookRequest bookRequest)
        {
            if(ModelState.IsValid)
            {
                bookRequest.BookRequestedBy = _userManager.GetUserId(User);
                _context.Add(bookRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(InfoMessage));
            }
            return View(bookRequest);
        }

        public ActionResult InfoMessage()
        {
            return View();
        }

        public async Task<IActionResult> BookRequestList()
        {
            var bookRequestList = (from bookReq in _context.BookRequests
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
                                       BookName = bookReq.BookInventory.BookName,
                                       Status = bookReq.Status,
                                       ReturnBy = bookReq.ReturnBy
                                   }).ToListAsync();

            return View(await bookRequestList);
        }

        public async Task<IActionResult> SendResponse(int? id, string status)
        {
            var userId = _userManager.GetUserId(User);
            var userDetails = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            var userEmail = (from bookReq in _context.BookRequests
                                   join user in _context.Users on bookReq.BookRequestedBy equals user.Id into bookRequser
                                   from user in bookRequser.DefaultIfEmpty()
                                   where bookReq.BookRequestId == id
                                   select new BKViewModel { Email = user.Email, BookId = bookReq.BookId }).FirstOrDefault();

            var callApi = new EmailController(_webHostEnvironment, _configuration);
            var result = callApi.SendEmail(userEmail.Email, $"{userDetails.FirstName } {userDetails.LastName}", status);

            if(status == Enums.Status.Approve.ToString())
            {
                var bookData = _context.BookInventories.Where(x => x.BookId == userEmail.BookId).FirstOrDefault();
                bookData.Quantity = bookData.Quantity - 1;
                bookData.Created = bookData.Created;
                bookData.CreatedBy = bookData.CreatedBy;
                bookData.LastModified = DateTime.Now;
                bookData.LastModifiedBy = _userManager.GetUserId(User);
                _context.Update(bookData);
            }

            var data = _context.BookRequests.Where(x => x.BookRequestId == id).FirstOrDefault();
            data.Status = status;
            _context.Update(data);
            await _context.SaveChangesAsync();

            return View();
        }

        // GET: BookInventories/Edit/5
        public ActionResult EditRequest(int? id)
        {
            var bookRequest = (from bookReq in _context.BookRequests
                                join user in _context.Users on bookReq.BookRequestedBy equals user.Id into bookRequser
                                from user in bookRequser.DefaultIfEmpty()
                                where bookReq.BookRequestId == id                
                                select new BookRequestViewModel
                                {
                                    BookRequestId = bookReq.BookRequestId,
                                    BookRequestedBy = bookReq.BookRequestedBy,
                                    Fullname = $"{user.FirstName} {user.LastName}",
                                    FromDate = bookReq.FromDate,
                                    ToDate = bookReq.ToDate,
                                    BookId = bookReq.BookId,
                                    BookName = bookReq.BookInventory.BookName,
                                    Status = bookReq.Status,
                                    ReturnBy = bookReq.ReturnBy
                                }).FirstOrDefault();
            return View(bookRequest);
        }

        // POST: BookInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRequest(BookRequestViewModel bookRequestViewModel)
        {
            if (ModelState.IsValid)
            {
                var bookReq = await _context.BookRequests.FindAsync(bookRequestViewModel.BookRequestId);
                bookReq.ReturnBy = bookRequestViewModel.ReturnBy;
                bookReq.Status = Enums.Status.Returned.ToString();
                _context.Update(bookReq);
               
                var bookData = _context.BookInventories.Where(x => x.BookId == bookRequestViewModel.BookId).FirstOrDefault();
                bookData.Quantity = bookData.Quantity + 1;
                bookData.Created = bookData.Created;
                bookData.CreatedBy = bookData.CreatedBy;
                bookData.LastModified = DateTime.Now;
                bookData.LastModifiedBy = _userManager.GetUserId(User);
                _context.Update(bookData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(BookRequestList));
            }
            return View();
        }

    }
}