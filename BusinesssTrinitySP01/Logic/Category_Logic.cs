﻿using BusinesssTrinitySP01.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Logic
{
    public class Category_Logic
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<Category> all()
        {
            return db.Categories.ToList();
        }
        public bool add(Category model)
        {
            try
            {
                db.Categories.Add(model);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public bool edit(Category model)
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
        public bool delete(Category model)
        {
            try
            {
                db.Categories.Remove(model);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public Category find_by_id(int? id)
        {
            return db.Categories.Find(id);
        }
        public List<Equipment> category_items(int? id)
        {
            return find_by_id(id).Equipments.ToList();
        }
    }
}