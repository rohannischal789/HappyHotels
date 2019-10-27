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
            var userID = User.Identity.GetUserId();
            if (!User.IsInRole("ADMIN")) // if user is a customer, fetch only his/her records
            {
                var bookings = db.Bookings.Where(b => b.user_id == userID).Include(b => b.Coupon).Include(b => b.HotelRoom);
                return View(bookings.ToList());
            }
            else
            {
                var bookings = db.Bookings.Include(b => b.Coupon).Include(b => b.HotelRoom);
                return View(bookings.ToList());
            }
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
            ViewBag.error = false;
            Session["hotelID"] = hotelID;
            model.HotelRoom = new HotelRoom();
            var comparingRoomID = model.hotelroom_id == 0 ? 1 : model.hotelroom_id;
            var room = db.Rooms.FirstOrDefault(r => r.room_id == comparingRoomID);
            var hotelRoom = db.HotelRooms.FirstOrDefault(h => h.hotel_id == hotelID && h.Room.room_name.ToLower() == room.room_name.ToLower());
            model.HotelRoom.approx_price = hotelRoom.approx_price; // setting up the price per day for the room
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
            int hotelID = Convert.ToInt32( Session["hotelID"]);
            booking.hotelroom_id = db.HotelRooms.FirstOrDefault(h => h.hotel_id == hotelID && h.room_id == booking.hotelroom_id).hotelroom_id; // updating the hotelroom_id as per the selected room id in hotel

            // if booking exists on the same day for the same room, booking constraint is shown
            if (db.Bookings.Any(b => b.hotelroom_id == booking.hotelroom_id && b.check_in_date == booking.check_in_date && b.check_out_date == booking.check_out_date))
            {
                ViewBag.error = true;
            }
            else if (ModelState.IsValid)
            {
                int dayDiff = (booking.check_out_date - booking.check_in_date).Days;
                if(dayDiff == 0 )
                {
                    dayDiff = 1;
                }
                booking.total_price = dayDiff * db.HotelRooms.FirstOrDefault(b => b.hotelroom_id == booking.hotelroom_id).approx_price; // calculate the total price
                db.Bookings.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.coupon_id = new SelectList(db.Coupons, "coupon_id", "coupon_code", booking.coupon_id);
            ViewBag.hotelroom_id = new SelectList(db.HotelRooms, "Room", "photo_link", booking.hotelroom_id);
            return View(booking);
        }

        [Authorize]
        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.error = false;

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
            booking.user_id = User.Identity.GetUserId();
            int hotelID = db.Bookings.FirstOrDefault(b=>b.booking_id==booking.booking_id).HotelRoom.hotel_id;
            booking.hotelroom_id = db.HotelRooms.FirstOrDefault(h => h.hotel_id == hotelID && h.room_id == booking.hotelroom_id).hotelroom_id; // updating the hotelroom_id as per the selected room id in hotel

            // if booking exists on the same day for the same room (apart from the current booking id), booking constraint is shown
            if (db.Bookings.Any(b => b.hotelroom_id == booking.hotelroom_id && b.check_in_date == booking.check_in_date && b.check_out_date == booking.check_out_date && b.booking_id != booking.booking_id))
            {
                ViewBag.error = true;
            }
            else if(ModelState.IsValid)
            {
                int dayDiff = (booking.check_out_date - booking.check_in_date).Days;
                if (dayDiff == 0)
                {
                    dayDiff = 1;
                }
                booking.total_price = dayDiff * db.HotelRooms.FirstOrDefault(b => b.hotelroom_id == booking.hotelroom_id).approx_price;  // calculate the total price

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

        [Authorize]
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
