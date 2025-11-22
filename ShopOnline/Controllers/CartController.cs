using ShopOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

    namespace ShopOnline.Controllers
    {
        public class CartController : Controller
        {
            // GET: GioHang

            QLBH2025Entities db = new QLBH2025Entities();
            // GET: HienThiCart, chuẩn bị dữ liệu cho View
            public ActionResult HienThiCart()
            {
                if (Session["Cart"] == null)
                    return View("HienThiCart");
                Cart _cart = Session["Cart"] as Cart;
                return View(_cart);
            }
            // Tạo mới giỏ hàng, nguồn được lấy từ Session
            public Cart GetCart()
            {
                Cart cart = Session["Cart"] as Cart;
                if (cart == null || Session["Cart"] == null)
                {
                    cart = new Cart();
                    Session["Cart"] = cart;
                }
                return cart;
            }
            // Thêm sản phẩm vào giỏ hàng
            public ActionResult AddToCart(int ProductID)
            {
                // lấy sản phẩm theo id
                var _pro = db.Products.SingleOrDefault(s => s.ProductID == ProductID);
                if (_pro != null)
                {
                    GetCart().Add_Product_Cart(_pro);
                }
                return RedirectToAction("HienThiCart", "Cart");
            }
            // Cập nhật số lượng và tính lại tổng tiền
            public ActionResult Update_Cart_Quantity(FormCollection form)
            {
                Cart cart = Session["Cart"] as Cart;
                int id_pro = int.Parse(Request.Form["idPro"]);
                int _quantity = int.Parse(Request.Form["carQuantity"]);
                cart.Update_quantity(id_pro, _quantity);

                return RedirectToAction("HienThiCart", "Cart");
            }
            // Xóa dòng sản phẩm trong giỏ hàng
            public ActionResult RemoveCart(int id)
            {
                Cart cart = Session["Cart"] as Cart;
                cart.Remove_CartItem(id);

                return RedirectToAction("HienThiCart", "Cart");
            }

            // Tính tổng tiền đơn hàng
            public PartialViewResult TongTienCart()
            {
                decimal total_money_item = 0;
                Cart cart = Session["Cart"] as Cart;
                if (cart != null)
                    total_money_item = cart.Total_money();
                ViewBag.TotalCart = total_money_item;
                return PartialView("TongTienCart");
            }

            // Tính tổng sản phẩm đơn hàng
            public PartialViewResult TongSanPhamCart()
            {
                int total_items = 0;
                Cart cart = Session["Cart"] as Cart;
                if (cart != null)
                    total_items = cart.Total_quantity(); // Gọi hàm đếm tổng số lượng

                ViewBag.TotalCart = total_items;
                return PartialView("TongSanPhamCart");
            }
            // Các phương thức cho đặt hàng thành công
            public ActionResult CheckOut(FormCollection form)
            {
                try
                {
                    Cart cart = Session["Cart"] as Cart;
                    OrderPro _order = new OrderPro();
                    _order.DateOrder = DateTime.Now;
                    _order.AddressDeliverry = form["AddressDeliverry"];
                    _order.IDCus = int.Parse(form["CodeCustomer"]);
                    _order.Status = "Chưa xác nhận";
                    db.OrderProes.Add(_order);
                    db.SaveChanges();
                    foreach (var item in cart.Items)
                    {
                        // lưu dòng sản phẩm vào chi tiết hóa đơn
                        OrderDetail _order_detail = new OrderDetail();
                        _order_detail.IDOrder = _order.ID;   // ✔ giờ ID Order đã có thật
                        _order_detail.IDProduct = item._product.ProductID;
                        _order_detail.UnitPrice = (double)item._product.Price;
                        _order_detail.Quantity = item._quantity;

                        db.OrderDetails.Add(_order_detail);
                    }
                    db.SaveChanges();
                    
                    return RedirectToAction("CheckOut_Success", "Cart");
                }
                catch
                {
                    return Content("Có sai sót! Xin kiểm tra lại thông tin"); ;
                }
            }

            //Đơn Hang Cua ban
            public ActionResult DonHangCuaBan()
            {
                // 1. Kiểm tra đã login chưa
                if (Session["IDCus"] == null)
                {
                    return RedirectToAction("Login", "User");
                }

                int idCus = (int)Session["IDCus"];

                // 2. Lấy tất cả đơn hàng của khách
                var orders = db.OrderProes
                    .Where(x => x.IDCus == idCus)
                    .OrderByDescending(x => x.DateOrder)
                    .ToList();

                return View(orders);
            }

            //Xác nhận đơn hàng
            public ActionResult XacNhanDonHang()
            {
                var account = Session["Account"] as Customer;

                if (account == null)
                    return RedirectToAction("Login", "User"); // nếu chưa login thì bắt đăng nhập


                return View(account);
            }

       


            // Thông báo thanh toán thành công
            public ActionResult CheckOut_Success()
            {
                return View();
            }
        }
    }