using ShopOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopOnline.Controllers
{
    public class UserController : Controller
    {
        QLBH2025Entities db = new QLBH2025Entities();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Customer cus)
        {
            if (ModelState.IsValid) 
            {
                
                var account = db.Customers.FirstOrDefault(k => k.EmailCus == cus.EmailCus && k.PasswordUser == cus.PasswordUser);
                if (account != null)
                {
                    
                    Session["Account"] = account;
                    Session["IDCus"] = account.IDCus;
                    return RedirectToAction("Index", "TrangChu");
                }
                else
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Customer customer, string rePassword)
        {
            // 1) Check confirm password
            if (customer.PasswordUser != rePassword)
            {
                ViewBag.Notification = "Mật khẩu xác nhận không chính xác";
                return View(customer);
            }

            // 2) Check email trùng
            var existed = db.Customers.FirstOrDefault(k => k.EmailCus == customer.EmailCus);
            if (existed != null)
            {
                ViewBag.NotificationEmail = "Đã có người đăng ký email này";
                return View(customer);
            }

            // 3) Lưu DB
            db.Customers.Add(customer);
            db.SaveChanges();

            // 4) Redirect về trang Login
            return RedirectToAction("Login", "User");
        }



        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login","User");
        }
        
    }
}