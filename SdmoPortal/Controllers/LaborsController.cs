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
    public class LaborsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Labors
        [ChildActionOnly]
        public ActionResult Index(int workOrderId, bool? readOnly)
        {
            //var labors = db.Labors.Include(l => l.WorkOrder);
            //return View(await labors.ToListAsync());
            ViewBag.WorkOrderId = workOrderId;

            var labors =  db.Labors.
                Where(l => l.WorkOrderId == workOrderId).
                OrderBy(l => l.ServiceItemCode);

            string partialViewName = readOnly == true ? "_IndexReadOnly" : "_Index";
            return PartialView(partialViewName, labors.ToList());
        }

        // GET: Labors/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labor labor = await db.Labors.FindAsync(id);
            if (labor == null)
            {
                return HttpNotFound();
            }
            return View(labor);
        }

        // GET: Labors/Create
        public ActionResult Create(int workOrderId)
        {
            Labor labor = new Labor();
            labor.WorkOrderId = workOrderId;
            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", labor.WorkOrderId);
            return PartialView("_Create", labor);
        }

        // POST: Labors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LaborId,WorkOrderId,ServiceItemCode,ServiceItemName,LaborHours,Rate,PercentComplete,ExtendedPrice,Notes")] Labor labor)
        {
            if (ModelState.IsValid)
            {
                db.Labors.Add(labor);
                await db.SaveChangesAsync();
                Log4NetHelper.Log(String.Format("Labor item {0} has been added to work order {1}.", labor.ServiceItemCode, labor.WorkOrderId), LogLevel.INFO, "WorkOrders", labor.WorkOrderId, User.Identity.Name, null);
                return Json(new { success = true }); //    RedirectToAction("Index");
            }

            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", labor.WorkOrderId);
            return PartialView("_Create", labor);
        }

        // GET: Labors/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labor labor = await db.Labors.FindAsync(id);
            if (labor == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", labor.WorkOrderId);
            return PartialView("_Edit", labor);
        }

        // POST: Labors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LaborId,WorkOrderId,ServiceItemCode,ServiceItemName,LaborHours,Rate,PercentComplete,ExtendedPrice,Notes")] Labor labor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(labor).State = EntityState.Modified;
                await db.SaveChangesAsync();
                Log4NetHelper.Log(String.Format("Labor item {0} has been edited for work order {1}.", labor.ServiceItemCode, labor.WorkOrderId), LogLevel.INFO, "WorkOrders", labor.WorkOrderId, User.Identity.Name, null);
                return Json(new { success = true });  // RedirectToAction("Index");
            }
            ViewBag.WorkOrderId = new SelectList(db.WorkOrders, "WorkOrderId", "Description", labor.WorkOrderId);
            return PartialView("_Edit", labor);
        }

        // GET: Labors/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Labor labor = await db.Labors.FindAsync(id);
            if (labor == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", labor);
        }

        // POST: Labors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Labor labor = await db.Labors.FindAsync(id);
            db.Labors.Remove(labor);
            await db.SaveChangesAsync();
            return Json(new { success = true }); //RedirectToAction("Index");
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
