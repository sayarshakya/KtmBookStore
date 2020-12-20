using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.MVC.Models
{
    public class BookInventory : CommonEntity
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        public string BookName { get; set; }
        public int? Quantity { get; set; }
    }
}
