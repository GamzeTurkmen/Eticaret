using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ETicaretProje.Models;
using Microsoft.AspNet.Identity;

namespace ETicaretProje.Controllers
{
    [Authorize] //giriş yapmış kişilerin buraya erişimi olması için
    public class SepetsController : Controller
    {
        private ETicaretEntities1 db = new ETicaretEntities1();

        public ActionResult SepeteEkle(int? adet, int id)
        {
            string userID = User.Identity.GetUserId();

            Sepet sepettekiUrun = db.Sepet.FirstOrDefault(a => a.RefUrunID == id && a.RefAspNetUserID == userID);
            Urunler urun = db.Urunler.Find(id);

            if (sepettekiUrun==null)
            {
                Sepet yeniUrun = new Sepet()
                {
                    RefAspNetUserID = userID,
                    RefUrunID = id,
                    Adet = adet ?? 1, //bir adet girildiyse adeti al, girilmediyse 1 al. (0 olmamalı min 1 olmalı)
                    ToplamTutar = Convert.ToDecimal((adet ?? 1) * urun.UrunFiyati)
                };
                db.Sepet.Add(yeniUrun);
            }
            else
            {
                sepettekiUrun.Adet = sepettekiUrun.Adet + (adet ?? 1);
                sepettekiUrun.ToplamTutar = Convert.ToDecimal(sepettekiUrun.Adet * urun.UrunFiyati);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult SepeteGuncelle(int? adet, int id)
        {
            Sepet sepet = db.Sepet.Find(id);
            Urunler urun = db.Urunler.Find(sepet.RefUrunID);

            sepet.Adet = adet ?? 1;
            sepet.ToplamTutar = Convert.ToDecimal(sepet.Adet * urun.UrunFiyati);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        public ActionResult Sil (int id)
        {
            Sepet sepet = db.Sepet.Find(id);
            db.Sepet.Remove(sepet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Sepets
        public ActionResult Index()
        {
            var sepet = db.Sepet.Include(s => s.AspNetUsers).Include(s => s.Urunler);
            return View(sepet.ToList());
        }

        // GET: Sepets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            return View(sepet);
        }

        // GET: Sepets/Create
        public ActionResult Create()
        {
            ViewBag.RefAspNetUserID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.RefUrunID = new SelectList(db.Urunler, "UrunID", "UrunAdi");
            return View();
        }

        // POST: Sepets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SepetID,RefAspNetUserID,RefUrunID,Adet,ToplamTutar")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                db.Sepet.Add(sepet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RefAspNetUserID = new SelectList(db.AspNetUsers, "Id", "Email", sepet.RefAspNetUserID);
            ViewBag.RefUrunID = new SelectList(db.Urunler, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // GET: Sepets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            ViewBag.RefAspNetUserID = new SelectList(db.AspNetUsers, "Id", "Email", sepet.RefAspNetUserID);
            ViewBag.RefUrunID = new SelectList(db.Urunler, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // POST: Sepets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SepetID,RefAspNetUserID,RefUrunID,Adet,ToplamTutar")] Sepet sepet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sepet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RefAspNetUserID = new SelectList(db.AspNetUsers, "Id", "Email", sepet.RefAspNetUserID);
            ViewBag.RefUrunID = new SelectList(db.Urunler, "UrunID", "UrunAdi", sepet.RefUrunID);
            return View(sepet);
        }

        // GET: Sepets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sepet sepet = db.Sepet.Find(id);
            if (sepet == null)
            {
                return HttpNotFound();
            }
            return View(sepet);
        }

        // POST: Sepets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sepet sepet = db.Sepet.Find(id);
            db.Sepet.Remove(sepet);
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
