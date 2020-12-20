using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserManagement.MVC.Data;
using UserManagement.MVC.Models;

namespace UserManagement.MVC.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class BookInventoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookInventoriesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BookInventories
        public async Task<IActionResult> Index()
        {
            var bookList = (from bookInv in _context.BookInventories
                            join user in _context.Users on bookInv.CreatedBy equals user.Id into bookInvuser
                            from user in bookInvuser.DefaultIfEmpty()
                            join userMd in _context.Users on bookInv.LastModifiedBy equals userMd.Id into bookInvuserMd
                            from userMd in bookInvuserMd.DefaultIfEmpty()
                            select new BookInventoryViewModel
                            {
                               BookId = bookInv.BookId,
                               BookName = bookInv.BookName,
                               Quantity = bookInv.Quantity,
                               CreatedBy = $"{user.FirstName} {user.LastName}",
                               LastModifiedBy = $"{userMd.FirstName} {userMd.LastName}",
                               Created = bookInv.Created,
                               LastModified = bookInv.LastModified
                            }).ToListAsync();

            return View(await bookList);
        }

        // GET: BookInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookInventory = await _context.BookInventories
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookInventory == null)
            {
                return NotFound();
            }

            return View(bookInventory);
        }

        // GET: BookInventories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BookInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookName,CreatedBy,Created,LastModifiedBy,LastModified,Quantity")] BookInventory bookInventory)
        {
            if (ModelState.IsValid)
            {
                bookInventory.Created = DateTime.Now;
                bookInventory.CreatedBy = _userManager.GetUserId(User);
                _context.Add(bookInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bookInventory);
        }

        // GET: BookInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookInventory = await _context.BookInventories.FindAsync(id);
            if (bookInventory == null)
            {
                return NotFound();
            }
            return View(bookInventory);
        }

        // POST: BookInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookName,CreatedBy,Created,LastModifiedBy,LastModified,Quantity")] BookInventory bookInventory)
        {
            if (id != bookInventory.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var book = await _context.BookInventories.AsNoTracking().FirstOrDefaultAsync(x => x.BookId == id);
                    bookInventory.Created = book.Created;
                    bookInventory.CreatedBy = book.CreatedBy;
                    bookInventory.LastModified = DateTime.Now;
                    bookInventory.LastModifiedBy = _userManager.GetUserId(User);
                    _context.Update(bookInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookInventoryExists(bookInventory.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bookInventory);
        }

        // GET: BookInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookInventory = await _context.BookInventories
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (bookInventory == null)
            {
                return NotFound();
            }

            return View(bookInventory);
        }

        // POST: BookInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookInventory = await _context.BookInventories.FindAsync(id);
            _context.BookInventories.Remove(bookInventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookInventoryExists(int id)
        {
            return _context.BookInventories.Any(e => e.BookId == id);
        }
    }
}
