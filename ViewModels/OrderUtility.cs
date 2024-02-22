using CRM.Models;

namespace CRM.ViewModels
{
    public static class OrderUtility
    {
        public static string GetPerfumeTitle(Item item, OrderItem orderItem)
        {
            if (orderItem != null)
            {
                return item?.ItemTitleEn ?? "";
            }

            return "";
        }
    }
}
