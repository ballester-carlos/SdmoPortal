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
using Microsoft.AspNet.Identity;

namespace SdmoPortal.Controllers
{
    public class WidgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Widgets
        public async Task<ActionResult> Index()
        {
            var widgets = db.Widgets;
            return View(await widgets.ToListAsync());
        }

        // GET: Widgets/Create
        public ActionResult Create()
        {
            //ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            return View();
        }

        // POST: Widgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "WidgetId,Description,MainBusCode,TestPassDateTime,WidgetStatus,CurrentWorkerId")] Widget widget)
        {
            if (ModelState.IsValid)
            {
                db.Widgets.Add(widget);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "WorkList");
            }

            ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", widget.CurrentWorkerId);
            return View(widget);
        }

        // GET: Widgets/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Widget widget = await db.Widgets.FindAsync(id);
            if (widget == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", widget.CurrentWorkerId);

            if (widget.Status.Substring(widget.Status.Length - 3, 3) != "ing")
                return View("Claim", widget);

            return View(widget.Status, widget);
        }

        // POST: Widgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "WidgetId,Description,MainBusCode,TestPassDateTime,WidgetStatus,CurrentWorkerId")] Widget widget, string command)
        {
            if (ModelState.IsValid)
            {
                PromotionResult promotionResult = new PromotionResult();
                if(command == "Save")
                {
                    promotionResult.Success = true;
                    promotionResult.Message = String.Format("Changes to widget {0} have been successfully saved.", widget.WidgetId);
                    Log4NetHelper.Log(promotionResult.Message, LogLevel.INFO, widget.EntityFormalNamePlural, widget.WidgetId, User.Identity.Name, null);
                }
                else if(command == "Claim")
                {
                    promotionResult = widget.ClaimWorkListItem(User.Identity.GetUserId());
                }
                else if (command == "Relinquish")
                {
                    promotionResult = widget.RelinquishWorkListItem();
                }
                else
                {
                    promotionResult = widget.PromoteWorkListItem(command);
                }

                if(promotionResult.Success)
                {
                    TempData["MessageToClient"] = promotionResult.Message;
                }

                db.Entry(widget).State = EntityState.Modified;
                await db.SaveChangesAsync();

                if(command == "Claim" && promotionResult.Success)
                {
                    return RedirectToAction("Edit", widget.WidgetId);
                }
                return RedirectToAction("Index","WorkList");
            }
            //ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", widget.CurrentWorkerId);
            return View(widget);
        }

        // GET: Widgets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Widget widget = await db.Widgets.FindAsync(id);
            if (widget == null)
            {
                return HttpNotFound();
            }
            return View(widget);
        }

        // POST: Widgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Widget widget = await db.Widgets.FindAsync(id);
            db.Widgets.Remove(widget);
            await db.SaveChangesAsync();
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
