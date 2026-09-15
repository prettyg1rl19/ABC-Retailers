using ST10438767_CLDV6212.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ST10438767_CLDV6212.Logic
{
    public class ShoppingCartActions
    {
        //    private ApplicationDbContext _db;
        //    private readonly IHttpContextAccessor _httpContextAccessor;
        //    public string ShoppingCartId { get; set; }
        //    public const string CartSessionKey = "CartId";

        //    public ShoppingCartActions(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        //    {
        //        _db = db;
        //        _httpContextAccessor = httpContextAccessor;
        //    }

        //    public void AddToCart(int productId)
        //    {
        //        ShoppingCartId = GetCartId();

        //        var cartItem = _db.ShoppingCartItems
        //            .SingleOrDefault(c => c.CartId == ShoppingCartId && c.Product_ID == productId);

        //        if (cartItem == null)
        //        {
        //            var product = _db.Products.SingleOrDefault(p => p.Product_ID == productId);
        //            if (product == null) throw new Exception("Product not found.");

        //            cartItem = new CartItem
        //            {
        //                ItemId = Guid.NewGuid().ToString(),
        //                Product_ID = productId,
        //                CartId = ShoppingCartId,
        //                Product = product,
        //                Quantity = 1,
        //                DateCreated = DateTime.Now
        //            };

        //            _db.ShoppingCartItems.Add(cartItem);
        //        }
        //        else
        //        {
        //            cartItem.Quantity++;
        //        }

        //        _db.SaveChanges();
        //    }

        //    public List<CartItem> GetCartItems()
        //    {
        //        ShoppingCartId = GetCartId();

        //        return _db.ShoppingCartItems
        //            .Include(c => c.Product)
        //            .Where(c => c.CartId == ShoppingCartId)
        //            .ToList();
        //    }

        //    public string GetCartId()
        //    {
        //        var session = _httpContextAccessor.HttpContext.Session;

        //        if (session.GetString(CartSessionKey) == null)
        //        {
        //            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;

        //            if (!string.IsNullOrWhiteSpace(userName))
        //            {
        //                session.SetString(CartSessionKey, userName);
        //            }
        //            else
        //            {
        //                var tempCartId = Guid.NewGuid().ToString();
        //                session.SetString(CartSessionKey, tempCartId);
        //            }
        //        }

        //        return session.GetString(CartSessionKey);
        //    }

        //    public void Dispose()
        //    {
        //        if (_db != null)
        //        {
        //            _db.Dispose();
        //            _db = null;
        //        }
        //    }
    }
}


/*
Reitan, E., 2025. Shopping Cart. [Online] Available at: 
< https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart > [Accessed 12 November 2025].
*/
