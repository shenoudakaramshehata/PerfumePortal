using System;
using System.Collections.Generic;


namespace CRM.Models
{
    public partial class Newsletter
    {
        public int NewsletterId { get; set; }
        public string Email { get; set; }
        public DateTime? Date { get; set; }


    }
}