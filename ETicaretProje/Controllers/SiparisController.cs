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
    public class SiparisController : Controller
    {
        private ETicaretEntities1 db = new ETicaretEntities1();

        // GET: Siparis
        public ActionResult Index()
        {
            var siparis = db.Siparis.Include(s => s.AspNetUsers);
            return View(siparis.ToList());
        }

        public ActionResult SiparisDetay(int id)
        {
            var siparisDetay = db.SiparisKalem.Where(a => a.RefSiparisID == id).ToList();
            return View(siparisDetay);
        }

        public ActionResult SiparisTamamla()
        {
            string userID = User.Identity.GetUserId();
            IEnumerable<Sepet> sepetUrunleri = db.Sepet.Where(a => a.RefAspNetUserID == userID).ToList();

            string ClientId = "100300000"; //bankadan alınan mağaza kodu
            string Amount = sepetUrunleri.Sum(a => a.ToplamTutar).ToString();//sepetteki ürünlerin toplam fiyatı
            string Oid = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);//sipariş id oluşturuluyor. farklı olmalı
            string OnayUrl = "http://localhost:1539/Siparis/Tamamlandi";//Ödeme tamamlandığında bankadan verilerin geleceği url
            string HataURL = "http://localhost:1539/Siparis/Hatali";//Ödeme hata verdiğinde bankadan gelecek url
            string RDN = "SDADA";//HSH karşılaştırması için eklenen rastgele dize
            string StoreKey = "123456";//bankanın sanal pos sayfasından alınıyor

            string TransActionType = "Auth"; //sabit değişmiyor
            string Instalment = "";
            string HshStr = ClientId + Oid + Amount + OnayUrl + HataURL + TransActionType + Instalment + RDN + StoreKey;

            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] HashBytes = System.Text.Encoding.GetEncoding("ISO-8859-9").GetBytes(HshStr);
            byte[] InputBytes = sha.ComputeHash(HashBytes);
            string Hash = Convert.ToBase64String(InputBytes);

            ViewBag.ClientId = ClientId;
            ViewBag.Oid = Oid;
            ViewBag.okUrl = OnayUrl;
            ViewBag.failUrl = HataURL;
            ViewBag.TransActionType = TransActionType;
            ViewBag.RDN = RDN;
            ViewBag.Hash = Hash;
            ViewBag.Amount = Amount;
            ViewBag.StoreType = "3d_pay_hosting"; // Ödeme modelimiz biz buna göre anlatıyoruz 
            ViewBag.Description = "";
            ViewBag.XID = "";
            ViewBag.Lang = "tr";
            ViewBag.EMail = "destek@destek.com";
            ViewBag.UserID = "mdurmaz"; // bu id yi bankanın sanala pos ekranında biz oluşturuyoruz.
            ViewBag.PostURL = "https://entegrasyon.asseco-see.com.tr/fim/est3Dgate";
            return View();
        }
        [HttpPost]
        public ActionResult Tamamlandi()
        {
            Siparis siparis = new Siparis();
            string userID = User.Identity.GetUserId();
            siparis.Ad = Request.Form.Get("Ad");
            siparis.Soyad = Request.Form.Get("Soyad");
            siparis.Adres = Request.Form.Get("Adres");
            siparis.Tarih = DateTime.Now;
            siparis.TCKimlikNo = Request.Form.Get("TcKimlikNo");
            siparis.Telefon = Request.Form.Get("Telefon");
            siparis.Eposta = Request.Form.Get("Eposta");
            siparis.RefAspNetUserID = userID;

            IEnumerable<Sepet> sepettekiUrunler = db.Sepet.Where(a => a.RefAspNetUserID == userID).ToList();
            foreach(Sepet sepetUrunu in sepettekiUrunler)
            {
                SiparisKalem yeniKalem = new SiparisKalem()
                {
                    Adet = sepetUrunu.Adet,
                    ToplamTutar = sepetUrunu.ToplamTutar,
                    RefUrunID = sepetUrunu.RefUrunID
                };

                siparis.SiparisKalem.Add(yeniKalem);
                db.Sepet.Remove(sepetUrunu);
                
            }

            db.Siparis.Add(siparis);
            db.SaveChanges();
            return View();

        }
        public ActionResult Hatali()
        {
            ViewBag.Hata = Request.Form;
            return View();

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
