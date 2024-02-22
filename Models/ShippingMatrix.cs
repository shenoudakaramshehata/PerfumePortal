using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models
{
    public class ShippingMatrix
    {
        [Key]
        public int ShippingMatrixId { get; set; }

        public int CountryId { get; set; }
        public Country? Country { get; set; }
        public double FromWeight { get; set; }
        public double ToWeight { get; set; }
        public double AramexPrice { get; set; }
        public double ActualPrice { get; set; }



    }
}
