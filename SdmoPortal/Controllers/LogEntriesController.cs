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
    public class LogEntriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult LogEntries(string entityFormalNamePlural, int entityKeyValue)
        {
            IEnumerable<LogEntry> logEntries = db.LogEntries.Where(le => 
                le.EntityFormalNamePlural == entityFormalNamePlural && le.EntityKeyValue == entityKeyValue)
                .OrderByDescending(le => le.LogDate);

            return PartialView("_LogEntries", logEntries);
        }

        // GET: LogEntries
        public async Task<ActionResult> Index()
        {
            return View(await db.LogEntries.ToListAsync());
        }

        // GET: LogEntries/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogEntry logEntry = await db.LogEntries.FindAsync(id);
            if (logEntry == null)
            {
                return HttpNotFound();
            }
            return View(logEntry);
        }

        // GET: LogEntries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LogEntries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LogEntryId,LogDate,Logger,LogLevel,Thread,EntityFormalNamePlural,EntityKeyValue,UserName,Message,Exception")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
                db.LogEntries.Add(logEntry);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(logEntry);
        }

        // GET: LogEntries/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogEntry logEntry = await db.LogEntries.FindAsync(id);
            if (logEntry == null)
            {
                return HttpNotFound();
            }
            return View(logEntry);
        }

        // POST: LogEntries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LogEntryId,LogDate,Logger,LogLevel,Thread,EntityFormalNamePlural,EntityKeyValue,UserName,Message,Exception")] LogEntry logEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logEntry).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(logEntry);
        }

        // GET: LogEntries/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LogEntry logEntry = await db.LogEntries.FindAsync(id);
            if (logEntry == null)
            {
                return HttpNotFound();
            }
            return View(logEntry);
        }

        // POST: LogEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            LogEntry logEntry = await db.LogEntries.FindAsync(id);
            db.LogEntries.Remove(logEntry);
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
