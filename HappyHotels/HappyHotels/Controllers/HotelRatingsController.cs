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
    public class HotelRatingsController : Controller
    {
        private HappyHotelsEntities db = new HappyHotelsEntities();

        // GET: HotelRatings
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
            if (!User.IsInRole("ADMIN"))
            {
                var hotelRatings = db.HotelRatings.Where(b => b.user_id == userID).Include(h => h.Hotel);
                return View(hotelRatings.ToList());
            }
            else
            {
                var hotelRatings = db.HotelRatings.Include(h => h.Hotel);
                return View(hotelRatings.ToList());
            }
        }

        // GET: HotelRatings/Create
        public ActionResult Create(int hotelID)
        {
            ViewBag.error = false;
            var hotels = db.Hotels.ToList();
            if (hotelID != 0)
            {
                hotels = db.Hotels.Where(h => h.hotel_id == hotelID).ToList();
            }
            ViewBag.hotel_id = new SelectList(hotels, "hotel_id", "name");
            return View();
        }

        // POST: HotelRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "hotelrating_id,hotel_id,rating,review,user_id")] HotelRating hotelRating)
        {
            hotelRating.user_id = User.Identity.GetUserId();
            if(hotelRating.rating <= 0)
            {
                ViewBag.error = true;
            }
            else if (ModelState.IsValid && hotelRating.rating > 0)
            {
                db.HotelRatings.Add(hotelRating);
                db.SaveChanges();

                var hotel = db.Hotels.FirstOrDefault(h => h.hotel_id == hotelRating.hotel_id);
                var ratingAvg = hotel.HotelRatings.Average(h => h.rating);
                hotel.rating = (int)ratingAvg;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.hotel_id = new SelectList(db.Hotels, "hotel_id", "name", hotelRating.hotel_id);
            return View(hotelRating);
        }

        // GET: HotelRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelRating hotelRating = db.HotelRatings.Find(id);
            if (hotelRating == null)
            {
                return HttpNotFound();
            }
            return View(hotelRating);
        }

        // POST: HotelRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HotelRating hotelRating = db.HotelRatings.Find(id);
            db.HotelRatings.Remove(hotelRating);
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
