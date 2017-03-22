using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SdmoPortal.DataLayer;
using SdmoPortal.Models;

namespace SdmoPortal.Controllers
{
    public class PartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Parts
        [ChildActionOnly]
        public ActionResult Index(int workOrderId)
        {
            ViewBag.WorkOrderId = workOrderId;

            var parts = db.Parts.
                Where(p => p.WorkOrderId == workOrderId).
                OrderBy(p => p.InventoryItemCode);
                //Include(p => p.WorkOrder);

            return PartialView("_Index", parts.ToList());
        }

        // GET: Parts/Create
        public ActionResult Create(int workOrderId)
        {
            Part part = new Part();
            part.WorkOrderId = workOrderId;
            return PartialView("_Create", part);
            //TODO: ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description");
            //TODO: return View();
        }

        // POST: Parts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PartId,WorkOrderId,InventoryItemCode,InventoryItemName,Quantity,UnitPrice,Notes,IsInstalled")] Part part)
        {
            if (ModelState.IsValid)
            {
                db.Parts.Add(part);
                await db.SaveChangesAsync();
                //TODO: return RedirectToAction("Index");
                return Json(new { success = true });
            }
            //TODO: the below line is commented on the tutorial
            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", part.WorkOrderId);
            //TODO:  return View(part);
            return PartialView("_Create", part);
        }

        // GET: Parts/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = await db.Parts.FindAsync(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", part.WorkOrderId);
            //TODO:  return View(part);
            return PartialView("_Edit", part);
        }

        // POST: Parts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PartId,WorkOrderId,InventoryItemCode,InventoryItemName,Quantity,UnitPrice,Notes,IsInstalled")] Part part)
        {
            if (ModelState.IsValid)
            {
                db.Entry(part).State = EntityState.Modified;
                await db.SaveChangesAsync();
                //TODO: return RedirectToAction("Index");
                return Json(new { success = true });
            }
            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", part.WorkOrderId);
            return PartialView("_Edit", part);
        }

        // GET: Parts/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = await db.Parts.FindAsync(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            //TODO: return View(part);
            return PartialView("_Delete", part);
        }

        // POST: Parts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Part part = await db.Parts.FindAsync(id);
            db.Parts.Remove(part);
            await db.SaveChangesAsync();
            //TODO: return RedirectToAction("Index");
            return Json(new { success = true });
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
