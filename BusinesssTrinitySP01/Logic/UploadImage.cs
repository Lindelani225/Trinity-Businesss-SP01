using BusinesssTrinitySP01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BusinesssTrinitySP01.Logic
{
    public class UploadImage
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

            public int UploadImageInDataBase(HttpPostedFileBase file, HttpPostedFileBase file2, HttpPostedFileBase file3, Equipment equipment)
        {
            equipment.Image1 = ConvertToBytes(file);
            equipment.Image2 = ConvertToBytes(file2);
            equipment.Image3 = ConvertToBytes(file3);

            var equipitem = new Equipment
            {
                Name = equipment.Name,
                Description = equipment.Description,
                Quantity = equipment.Quantity,
                UnitCost = equipment.UnitCost,
                Rentprice = equipment.Rentprice,
                CategoryID = equipment.CategoryID,
                Image1 = equipment.Image1,
                Image2 = equipment.Image2,
                Image3 = equipment.Image3
                
            };
            db.Equipment.Add(equipitem);
            int i = db.SaveChanges();
            if (i == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }
}