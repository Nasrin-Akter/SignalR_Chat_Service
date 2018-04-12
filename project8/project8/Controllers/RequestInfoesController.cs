using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using project8.Models;
using System.IO;
using System.Drawing;
using Microsoft.AspNet.SignalR;

namespace project8.Controllers
{
    public class RequestInfoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        string gname = "";

        // GET: /RequestInfoes/
        public ActionResult Index()
        {
            var pending = db.RequestInfos.ToList();
            return View(pending);
        }

        public ActionResult PendingRequest()
        {
            var pending = db.RequestInfos.Where(r => r.Approved.Equals(false)).ToList();
            if (pending.Count > 0)
            {
                return View(pending);
            }
            else
            {
                ModelState.AddModelError("", "There is no request");
                return View(pending);
            }
        }

        // GET: /RequestInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestInfo requestinfo = db.RequestInfos.Find(id);
            if (requestinfo == null)
            {
                return HttpNotFound();
            }
            return View(requestinfo);
        }

        // GET: /RequestInfoes/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        // POST: /RequestInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult Create([Bind(Include="ID,GroupName,ReqDateTime,UserName,Approved")] RequestInfo requestinfo)
        {
            if (ModelState.IsValid)
            {
                var reqtoInsert = new RequestInfo
                {
                    Approved = false,
                    UserName = User.Identity.Name,
                    GroupName = requestinfo.GroupName,
                    ReqDateTime = DateTime.Now,
                };
                db.RequestInfos.Add(reqtoInsert);
                if (db.SaveChanges() > 0)
                {
                    return Json(new { success = true, data = reqtoInsert });
                }

            }

            return Json(new { success = false });
        }

        public virtual string UploadFiles(object obj)
        {
            gname = db.RequestInfos.Where(u => u.UserName.Equals(User.Identity.Name))
                    .Select(s => s.GroupName).SingleOrDefault();
            var hContext = GlobalHost.ConnectionManager.GetHubContext<MyHub>();
            var length = Request.ContentLength;
            var bytes = new byte[length];
            if (bytes.Length <= 15728640)
            {
                Request.InputStream.Read(bytes, 0, length);

                var fileName = Request.Headers["X-File-Name"];
                var fileSize = Request.Headers["X-File-Size"];
                var fileType = Request.Headers["X-File-Type"];
                var ipath = "/Images/" + fileName;
                var saveToFileLoc = HttpContext.Server.MapPath("~/Images/") + fileName;
                var original = HttpContext.Server.MapPath("~/Original Image/") + fileName;

                var fileStream = new FileStream(original, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(bytes, 0, length);
                fileStream.Close();

                string ext = Path.GetExtension(fileName);
                if(ext ==".jpg"||ext ==".png"||ext ==".jpeg"||ext ==".gif")
               {
                
                FileInfo gallaryFileInfo = new FileInfo(original);
                if (gallaryFileInfo.Name != null)
                {
                    Bitmap originalImage = new Bitmap(gallaryFileInfo.FullName);
                    double photoRatio = (double)originalImage.Height / (double)originalImage.Width;
                    int width = 120;
                    int height = (int)(width * photoRatio);
                    Size photosize = new Size(width, height);
                    Bitmap resizePhoto = new Bitmap(originalImage, photosize);
                    resizePhoto.SetResolution(72, 72);
                    resizePhoto.Save(saveToFileLoc);
                    resizePhoto.Dispose();
                    originalImage.Dispose();
                    hContext.Clients.Group(gname).received(User.Identity.Name, ipath, "files");
                }
                }                  
                else
                {

                    hContext.Clients.Group(gname).received(User.Identity.Name, "/Original Image/" + fileName, "files");
                }
                    db.MessageInfos.Add(new MessageInfo
                    {
                        MessageBody = ipath,
                        PostDateTime = DateTime.Now.ToString(),
                        UserName = User.Identity.Name

                    });

                    if (db.SaveChanges() > 0)
                    {
                        //hContext.Groups.Add()
                      
                        //hContext.Clients.All.receivedMessage("SERVER", ipath);
                        //hContext.Clients.AllExcept().receivedMessage("SERVER", ipath);
                    }
                

                return string.Format("{0} bytes uploaded", bytes.Length);
            }
            return string.Format("{0} bytes limit exceed", bytes.Length);
        }

        // GET: /RequestInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestInfo requestinfo = db.RequestInfos.Find(id);
            if (requestinfo == null)
            {
                return HttpNotFound();
            }
            return View(requestinfo);
        }

        // POST: /RequestInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,GroupName,ReqDateTime,UserName,Approved")] RequestInfo requestinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requestinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(requestinfo);
        }

        // GET: /RequestInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestInfo requestinfo = db.RequestInfos.Find(id);
            if (requestinfo == null)
            {
                return HttpNotFound();
            }
            return View(requestinfo);
        }

        // POST: /RequestInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RequestInfo requestinfo = db.RequestInfos.Find(id);
            db.RequestInfos.Remove(requestinfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetNotifications()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Notification nc = new Notification();
                DateTime dt = Convert.ToDateTime(Session["Last"]).Date;
                List<RequestInfo> lst = nc.GetStudents(dt);
                return Json(new { data = lst, success = true }, JsonRequestBehavior.AllowGet);
            }
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
