using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopOnline.Models;

namespace ShopOnline.Controllers
{
    public class OrderAdminController : Controller
    {
        private QLBH2025Entities db = new QLBH2025Entities();

        private bool IsAdminLoggedIn()
        {
            return Session["AdminUser"] != null;
        }

        public ActionResult Index()
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Admin");
            }

            var orders = db.OrderProes.Include(o => o.Customer).OrderByDescending(o => o.DateOrder).ToList();
            return View(orders);
        }

        public ActionResult Details(int? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderPro order = db.OrderProes.Include(o => o.Customer).Include(o => o.OrderDetails).FirstOrDefault(o => o.ID == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            foreach (var detail in order.OrderDetails)
            {
                detail.Product = db.Products.Find(detail.IDProduct);
            }

            decimal totalAmount = 0;
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Quantity.HasValue && detail.UnitPrice.HasValue)
                {
                    totalAmount += (decimal)(detail.Quantity.Value * detail.UnitPrice.Value);
                }
            }
            ViewBag.TotalAmount = totalAmount;

            return View(order);
        }

        public ActionResult Edit(int? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderPro order = db.OrderProes.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", order.IDCus);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DateOrder,IDCus,AddressDeliverry")] OrderPro order)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "NameCus", order.IDCus);
            return View(order);
        }

        public ActionResult Delete(int? id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Admin");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OrderPro order = db.OrderProes.Include(o => o.Customer).FirstOrDefault(o => o.ID == id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Admin");
            }

            OrderPro order = db.OrderProes.Find(id);

            var orderDetails = db.OrderDetails.Where(od => od.IDOrder == id).ToList();
            foreach (var detail in orderDetails)
            {
                db.OrderDetails.Remove(detail);
            }

            db.OrderProes.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
