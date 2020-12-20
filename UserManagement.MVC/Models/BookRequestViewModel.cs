using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.MVC.Models
{
    public class BookRequestViewModel
    {
        public int BookRequestId { get; set; }
        public string BookRequestedBy { get; set; }
        public string Fullname { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Status { get; set; }
        public string ReturnBy { get; set; }

    }

    public class BKViewModel
    {
        public string Email { get; set; } 
        public int BookId { get; set; }
    }
}
