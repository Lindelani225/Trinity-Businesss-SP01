using BusinesssTrinitySP01.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Logic
{
    public class Equipment_Logic
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Equipment> all()
        {
            return db.Equipment.Include(i => i.Category).ToList();
        }
        public bool add(Equipment model)
        {
            try
            {
                db.Equipment.Add(model);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public bool edit(Equipment model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public bool delete(Equipment model)
        {
            try
            {
                db.Equipment.Remove(model);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public Equipment find_by_id(int? id)
        {
            return db.Equipment.Find(id);
        }
        //public List<StockCart_Item> get_cart_items(int id)
        //{
        //    //return db.StockCart_Items.
        //}


        public void updateStock_Received(int item_id, int quantity)
        {
            var item = db.Equipment.Find(item_id);
            item.Quantity += quantity;
            db.SaveChanges();
        }
        public void updateOrder(int id, double price)
        {
            var item = db.OrderItems.Find(id);
            item.price = price;
            db.SaveChanges();
        }
    }
}