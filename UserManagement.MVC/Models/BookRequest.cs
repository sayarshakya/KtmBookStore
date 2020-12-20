using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.MVC.Models
{
    public class BookRequest
    {
        [Key]
        public int BookRequestId { get; set; }
        public string BookRequestedBy { get; set; }
        [Required]
        public DateTime FromDate { get; set; }
        [Required]
        public DateTime ToDate { get; set; }
        public int BookId { get; set; }
        [ForeignKey("BookId")]
        public virtual BookInventory BookInventory { get; set; }
        public string Status { get; set; }
        public string ReturnBy { get; set; }
    }
}
