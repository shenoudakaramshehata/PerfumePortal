using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models
{
    public partial class ContactUs
    {
        [Key]
        public int ContactId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Msg { get; set; }
        public DateTime? TransDate { get; set; }
    }
}
