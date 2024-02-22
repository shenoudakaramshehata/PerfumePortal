using CRM.Data;
using CRM.Models;

namespace CRM.Services
{
    public class OrderService
    {

   
        public string GetPerfumeTitle(OrderItem orderItem)
        {
            if (orderItem != null)
            {
                using (var context = new PerfumeContext())
                {
                    var item = context.Item.FirstOrDefault(e => e.ItemId == orderItem.ItemId);
                    return item?.ItemTitleEn ?? "";
                }
            }

            return "";
        }
    }
}
