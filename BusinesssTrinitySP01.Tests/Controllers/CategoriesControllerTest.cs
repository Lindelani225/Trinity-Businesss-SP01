using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinesssTrinitySP01.Controllers;
using BusinesssTrinitySP01.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinesssTrinitySP01.Tests.Controllers
{
    [TestClass]
    public class CategoriesControllerTest
    {
        private ApplicationDbContext db = new  ApplicationDbContext();

        //[TestMethod]
        //public void TestCreate()
        //{
        //    var controller = new CategoriesController();
        //    var result = controller.Create() as ViewResult;
        //    Assert.AreEqual("Create", result.ViewName);
        //}

        //[TestMethod]
        //public void TestIndex()
        //{
        //    var controller = new CategoriesController();
        //    var result = controller.Create() as ViewResult;
        //    var list = (List<Category>)result.ViewData.Model;
        //    var categories = db.Categories.ToList();
        //    Assert.AreEqual("Create", result.ViewName);
        //}
    }

}
