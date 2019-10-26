using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HappyHotels.Models;
using Microsoft.AspNet.Identity;


namespace HappyHotels.Controllers
{
    public class BookingsController : Controller
    {
        private HappyHotelsEntities db = new HappyHotelsEntities();

        [Authorize]
        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Coupon).Include(b => b.HotelRoom);
            return View(bookings.ToList());
        }

        [Authorize]
        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        [Authorize]
        // GET: Bookings/Create
        public ActionResult Create(int hotelID)
        {
            var model = new Booking();
            model.HotelRoom = new HotelRoom();
            var comparingRoomID = model.hotelroom_id == 0 ? 1 : model.hotelroom_id;
            var room = db.Rooms.FirstOrDefault(r => r.room_id == comparingRoomID);
            var hotelRoom = db.HotelRooms.FirstOrDefault(h => h.hotel_id == hotelID && h.Room.room_name.ToLower() == room.room_name.ToLower());
            model.HotelRoom.approx_price = hotelRoom.approx_price;
            ViewBag.HotelName = db.Hotels.FirstOrDefault(h => h.hotel_id == hotelID).name;
            ViewBag.coupon_id = new SelectList(db.Coupons, "coupon_id", "coupon_code");
            ViewBag.hotelroom_id = new SelectList(db.HotelRooms, "Room", "photo_link");
            return View(model);
        }

        [Authorize]
        // POST: Bookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "booking_id,user_id,hotelroom_id,check_in_date,check_out_date,no_of_adults,no_of_children,total_price,coupon_id")] Booking booking)
        {
            booking.user_id = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.coupon_id = new SelectList(db.Coupons, "coupon_id", "coupon_code", booking.coupon_id);
            ViewBag.hotelroom_id = new SelectList(db.HotelRooms, "Room", "photo_link", booking.hotelroom_id);
            ViewBag.isClicked = false;
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.coupon_id = new SelectList(db.Coupons, "coupon_id", "coupon_code", booking.coupon_id);
            ViewBag.hotelroom_id = new SelectList(db.HotelRooms, "hotelroom_id", "photo_link", booking.hotelroom_id);
            return View(booking);
        }

        [Authorize]
        // POST: Bookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "booking_id,user_id,hotelroom_id,check_in_date,check_out_date,no_of_adults,no_of_children,total_price,coupon_id")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.coupon_id = new SelectList(db.Coupons, "coupon_id", "coupon_code", booking.coupon_id);
            ViewBag.hotelroom_id = new SelectList(db.HotelRooms, "hotelroom_id", "photo_link", booking.hotelroom_id);
            return View(booking);
        }

        [Authorize]
        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
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
