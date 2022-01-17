using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinesssTrinitySP01.Models;

namespace BusinesssTrinitySP01.Logic
{
    public class Client_Logic
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string shoppingCartID { get; set; }
        public const string CartSessionKey = "CartId";


        public string GetCartID()
        {
            if (System.Web.HttpContext.Current.Session[name: CartSessionKey] == null)
            {
                if (!String.IsNullOrWhiteSpace(value: System.Web.HttpContext.Current.User.Identity.Name))
                {
                    System.Web.HttpContext.Current.Session[name: CartSessionKey] = System.Web.HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    Guid temp = Guid.NewGuid();
                    System.Web.HttpContext.Current.Session[name: CartSessionKey] = temp.ToString();
                }
            }
            return System.Web.HttpContext.Current.Session[name: CartSessionKey].ToString();
        }

        public void AddToCart(int id)
        {
            shoppingCartID = GetCartID();

            var item = db.Equipment.Find(id);
            if (item != null)
            {
                var cartItem =
                    db.CartItems.FirstOrDefault(x => x.CartID == shoppingCartID && x.EquipmentID == item.EquipmentID);
                if (cartItem == null)
                {
                    var cart = db.carts.Find(shoppingCartID);
                    if (cart == null)
                    {
                        db.carts.Add(entity: new Cart()
                        {
                            CartId = shoppingCartID,
                            Date_Created = DateTime.Now
                        });
                        db.SaveChanges();
                    }

                    db.CartItems.Add(entity: new CartItem()
                    {
                        ItemID = Guid.NewGuid().ToString(),
                        CartID = shoppingCartID,
                        EquipmentID = item.EquipmentID,
                        Date = DateTime.Now,
                        Quantity = 1,
                        price = item.Rentprice
                    }
                        );
                }
                else
                {
                    cartItem.Quantity++;
                }
                db.SaveChanges();
            }
        }

        public void RemoveFromCart(string id)
        {
            shoppingCartID = GetCartID();

            var item = db.CartItems.Find(id);
            if (item != null)
            {
                var cartItem =
                    db.CartItems.FirstOrDefault(predicate: x => x.CartID == shoppingCartID && x.ItemID == item.ItemID);
                if (cartItem != null)
                {
                    db.CartItems.Remove(entity: cartItem);
                }
                db.SaveChanges();
            }
        }

        public void EmptyCart()
        {
            shoppingCartID = GetCartID();
            foreach (var item in db.CartItems.ToList().FindAll(match: x => x.CartID == shoppingCartID))
            {
                db.CartItems.Remove(item);
            }
            try
            {
                db.carts.Remove(db.carts.Find(shoppingCartID));
                db.SaveChanges();
            }
            catch (Exception ex) { }
        }

        public List<CartItem> GetCartItems()
        {
            shoppingCartID = GetCartID();
            return db.CartItems.ToList().FindAll(match: x => x.CartID == shoppingCartID);
        }


        public void UpdateCart(string id, int qty)
        {
            var item = db.CartItems.Find(id);
            if (qty < 0)
                item.Quantity = qty / -1;
            else if (qty == 0)
                RemoveFromCart(item.ItemID);
            else
                item.Quantity = qty;
            db.SaveChanges();
        }

        public double GetCartTotal(string id)
        {
            double amount = 0;
            foreach (var item in db.CartItems.ToList().FindAll(match: x => x.CartID == id))
            {
                amount += (item.price * item.Quantity);
            }
            return amount;
        }

        public double GetPackageTotal(int id)
        {
            double amount = 0.0;
            foreach (var item in db.packageItems.ToList().FindAll(match: x => x.PckId == id))
            {
                amount += (item.Price * item.Qty);
            }
            return amount;
        }


        public double GetOrdertTotal(int id)
        {
            double amount = 0;
            foreach (var item in db.OrderItems.ToList().FindAll(match: x => x.OrderID == id))
            {
                amount += (item.price * item.quantity);
            }
            return amount;
        }

        public void UpdateStock(int id)
        {
            var order = db.orders.Find(id);
            List<OrderItem> items = db.OrderItems.ToList().FindAll(x => x.OrderID == id);
            foreach (var item in items)
            {
                var product = db.Equipment.Find(item.EquipmentID);
                if (product != null)
                {
                    if ((product.Quantity -= item.quantity) >= 0)
                    {
                        product.Quantity -= item.quantity;
                    }
                    else
                    {
                        item.quantity = product.Quantity;
                        product.Quantity = 0;
                    }
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex) { }
                }
            }
        }

    }


}