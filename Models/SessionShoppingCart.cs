using CRM.Data;

namespace CRM.Models
{
    public class SessionShoppingCart
    {
        private PerfumeContext _PerfumeContext { get; set; }
        public string ShoppingCartId { get; set; }
        public List<ShoppingCart> ShoppingCartItems { get; set; }
        public SessionShoppingCart(PerfumeContext _PerfumeContext)
        {
            this._PerfumeContext = _PerfumeContext;
        }
        //public static SessionShoppingCart GetCart(IServiceProvider services)
        //{
        //    ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
        //    var context = services.GetService<PerfumeContext>();
        //    string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();
        //    session.SetString("CartId", cartId);
        //    return new SessionShoppingCart(context)
        //    {
        //        ShoppingCartId = cartId
        //    };
        //}

    }
}
