using ETicaretProje.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ETicaretProje.Controllers
{
    public class HomeController : Controller
    {
        private ETicaretEntities1 db = new ETicaretEntities1();

        public ActionResult Index()
        {
            ViewBag.KategoriListesi = db.Kategoriler.ToList();
            ViewBag.SonUrunler = db.Urunler.OrderByDescending(a => a.UrunID).Skip(0).Take(12).ToList();
            return View();
        }

        public ActionResult Kategori(int id)
        {
            ViewBag.KategoriListesi = db.Kategoriler.ToList();
            ViewBag.Kategori = db.Kategoriler.Find(id);
            return View(db.Urunler.Where(a=>a.RefKategoriID==id).OrderBy(a=>a.UrunAdi).ToList());
        }

        public ActionResult Urun(int id)
        {
            ViewBag.KategoriListesi = db.Kategoriler.ToList();
            return View(db.Urunler.Find(id));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}