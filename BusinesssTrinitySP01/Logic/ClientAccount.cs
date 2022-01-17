using BusinesssTrinitySP01.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Logic
{
    public class ClientAccount
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<ClientProfile> all()
        {
            return db.clientProfiles.ToList();
        }
        public bool add(ClientProfile model)
        {
            try
            {
                db.clientProfiles.Add(model);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            { return false; }
        }
        public bool edit(ClientProfile model)
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
        public ClientProfile find_by_id(int? id)
        {
            return db.clientProfiles.Find(id);
        }

        public string getGender(string id_num)
        {
            if (Convert.ToInt16(id_num.Substring(7, 1)) >= 5)
                return "Male";
            else
                return "Female";
        }
    }
}