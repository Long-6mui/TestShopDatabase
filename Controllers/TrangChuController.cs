using ShopOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ShopOnline.Controllers
{
    public class TrangChuController : Controller
    {
        QLBH2025Entities db = new QLBH2025Entities();

        // GET: /TrangChu/Index
        [HttpGet]
        public ActionResult Index()
        {
            var products = db.Products.Take(4).ToList();
            return View(products);
        }
        [HttpGet]
        public ActionResult Shop(string idCate)
        {

            // Lấy toàn bộ sản phẩm và danh mục
            var categories = db.Categories.ToList();
            var products = db.Products.Include("Category").ToList();
             
            // Nếu người dùng bấm vào một danh mục cụ thể
            if (!string.IsNullOrEmpty(idCate))
            {
                // So sánh ignore case và trim khoảng trắng
                products = products  
                    .Where(p => p.IDCate != null && p.IDCate.Trim().ToUpper() == idCate.Trim().ToUpper())
                    .ToList();
            }

            // Truyền danh mục xuống view
            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = idCate?.Trim().ToUpper(); // highlight active category
            // CurrentCategory danh mục đang chọn của user 
            return View(products);
        }

        [HttpGet]
        public ActionResult Bestseller(string idCate)
        {

            // Lấy toàn bộ sản phẩm và danh mục
            var categories = db.Categories.ToList();
            var products = db.Products.Include("Category").ToList();

            // Nếu người dùng bấm vào một danh mục cụ thể
            if (!string.IsNullOrEmpty(idCate))
            {
                // So sánh ignore case và trim khoảng trắng
                products = products
                    .Where(p => p.IDCate != null && p.IDCate.Trim().ToUpper() == idCate.Trim().ToUpper())
                    .ToList();
            }

            // Truyền danh mục xuống view
            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = idCate?.Trim().ToUpper(); // idCate?: idCate có thể null mà không lỗi , chir Trim() và ToUpper() nếu != null 
            // CurrentCategory danh mục đang chọn của user 
            return View(products);
        }

        [HttpGet]
        public ActionResult Collections(string idCate)
        {

            // Lấy toàn bộ sản phẩm và danh mục
            var categories = db.Categories.ToList();
            var products = db.Products.Include("Category").ToList();

            // Nếu người dùng bấm vào một danh mục cụ thể
            if (!string.IsNullOrEmpty(idCate))
            {
                // So sánh ignore case và trim khoảng trắng
                products = products
                    .Where(p => p.IDCate != null && p.IDCate.Trim().ToUpper() == idCate.Trim().ToUpper())
                    .ToList();
            }

            // Truyền danh mục xuống view
            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = idCate?.Trim().ToUpper(); // idCate?: idCate có thể null mà không lỗi , chir Trim() và ToUpper() nếu != null 
            // CurrentCategory danh mục đang chọn của user 
            return View(products);
        }


    }
}
