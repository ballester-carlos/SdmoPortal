﻿using System;
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
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.Owin;

namespace SdmoPortal.Controllers
{
    [Authorize]
    public class WorkOrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkOrders
        public async Task<ActionResult> Index()
        {
            //Log4NetHelper.Log("Hello sailor!", LogLevel.INFO, "TEST", 0, "Tester", null);

            var workOrders = db.WorkOrders.Include(w => w.CurrentWorker).Include(w => w.Customer);
            return View(await workOrders.ToListAsync());
        }

        // GET: WorkOrders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkOrder workOrder = await db.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return HttpNotFound();
            }
            return View(workOrder);
        }

        // GET: WorkOrders/Create
        public ActionResult Create()
        {
            //ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
            ViewBag.CustomerId = new SelectList(db.Customers.Where(c => c.Cloaked == false), "CustomerId", "CompanyName");
            return View();
        }

        // POST: WorkOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "WorkOrderId,CustomerId,OrderDateTime,TargetDateTime,DropDeadDateTime,Description,WorkOrderStatus,CertificationRequirements,CurrentWorkerId")] WorkOrder workOrder)
        {
            if (ModelState.IsValid)
            {
                workOrder.CurrentWorkerId = User.Identity.GetUserId();
                db.WorkOrders.Add(workOrder);
                await db.SaveChangesAsync();

                Log4NetHelper.Log(String.Format("Work order {0} created.",workOrder.WorkOrderId), LogLevel.INFO, "WorkOrders", workOrder.WorkOrderId, User.Identity.Name, null);

                return RedirectToAction("Edit", new { controller = "WorkOrders", action = "Edit", Id = workOrder.WorkOrderId });
            }

            //ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", workOrder.CurrentWorkerId);
            ViewBag.CustomerId = new SelectList(db.Customers.Where(c => c.Cloaked == false), "CustomerId", "AccountNumber", workOrder.CustomerId);
            return View(workOrder);
        }

        // GET: WorkOrders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WorkOrder workOrder = await db.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return HttpNotFound();
            }
            // if a different user has claimed the work order since you refreshed the work list, redirect to work list with error message
            if(workOrder.CurrentWorkerId != null && workOrder.CurrentWorkerId != User.Identity.GetUserId())
            {
                ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                string claimedUserName = userManager.FindById(workOrder.CurrentWorkerId).UserName;

                string message = String.Format("User {0} has claimed work order {1} before user {2} could, and so the work order remains claimed by {0}.", claimedUserName, workOrder.WorkOrderId, User.Identity.GetUserId()); //?????
                TempData["MessageToClient"] = message;
                Log4NetHelper.Log(message, LogLevel.INFO, workOrder.EntityFormalNamePlural, workOrder.WorkOrderId, User.Identity.Name, null);
                return RedirectToAction("Index", "WorkList");

            }

            if (workOrder.Status.Substring(workOrder.Status.Length - 3, 3) != "ing")
                return View("Claim", workOrder);

            return View(workOrder.Status, workOrder);
        }

        // POST: WorkOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "WorkOrderId,CustomerId,OrderDateTime,TargetDateTime,DropDeadDateTime,Description,WorkOrderStatus,CertificationRequirements,CurrentWorkerId,ReworkNotes,RowVersion")] WorkOrder workOrder, string command)
        {
            if (ModelState.IsValid)
            {
                workOrder.Parts = db.Parts.Where(p => p.WorkOrderId == workOrder.WorkOrderId).ToList();
                workOrder.Labors = db.Labors.Where(l => l.WorkOrderId == workOrder.WorkOrderId).ToList();

                PromotionResult pr = new PromotionResult();

                if (command == "Save")
                {
                    pr.Success = true;
                    pr.Message = String.Format("Changes to work order {0} have been successfully saved.", workOrder.Id);
                    Log4NetHelper.Log(pr.Message, LogLevel.INFO, workOrder.EntityFormalNamePlural, workOrder.Id, User.Identity.Name, null);
                }
                else if (command == "Claim")
                    pr = workOrder.ClaimWorkListItem(User.Identity.GetUserId());
                else if (command == "Relinquish")
                    pr = workOrder.RelinquishWorkListItem();
                else
                    pr = workOrder.PromoteWorkListItem(command);

                if(!pr.Success)
                    TempData["MessageToClient"] = pr.Message;

                //try
                //{
                //    var zero = 0;
                //    var test = 1 / zero;
                //}
                //catch (Exception ex)
                //{
                //    Log4NetHelper.Log(null, LogLevel.ERROR, "WorkOrders", workOrder.WorkOrderId, User.Identity.Name, ex);
                //}

                //Now is handle by the ClaimWorkOrder method
                //workOrder.CurrentWorkerId = User.Identity.GetUserId();
                db.Entry(workOrder).State = EntityState.Modified;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    if(command == "Claim")
                        TempData["MessageToClient"] = String.Format("Someone else has claimed work order {0} since you retrieve it.", workOrder.WorkOrderId);
                    else
                        TempData["MessageToClient"] = String.Format("Someone else has modified work order {0} since you retrieve it.", workOrder.WorkOrderId);
                    return RedirectToAction("Index", "WorkList");
                }
                Log4NetHelper.Log(pr.Message, LogLevel.INFO, workOrder.EntityFormalNamePlural, workOrder.Id, User.Identity.Name, null);
                if (command == "Claim" && pr.Success)
                    return RedirectToAction("Edit", workOrder.WorkOrderId);

                return RedirectToAction("Index", "WorkList");
            }
            //ViewBag.CurrentWorkerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", workOrder.CurrentWorkerId);
            //ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "AccountNumber", workOrder.CustomerId);
            return View(workOrder);
        }

        // GET: WorkOrders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            WorkOrder workOrder = await db.WorkOrders.FindAsync(id);

            if (workOrder == null)
                return HttpNotFound();

            return View(workOrder);
        }

        // POST: WorkOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            WorkOrder workOrder = await db.WorkOrders.FindAsync(id);
            db.WorkOrders.Remove(workOrder);
            await db.SaveChangesAsync();

            Log4NetHelper.Log(String.Format("Work order {0} deleted.", workOrder.WorkOrderId), LogLevel.INFO, "WorkOrders", workOrder.WorkOrderId, User.Identity.Name, null);
            
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
