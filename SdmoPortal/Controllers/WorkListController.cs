using System.Web.Mvc;
using SdmoPortal.DataLayer;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using SdmoPortal.Models;

namespace SdmoPortal.Controllers
{
    [Authorize]
    public class WorkListController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ApplicationUserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        // GET: WorkList
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            List<string> userRolesList = UserManager.GetRoles(userId).ToList();

            IEnumerable<IWorkListItem> workListItemsToDisplay = new List<IWorkListItem>();
            workListItemsToDisplay = workListItemsToDisplay.Concat(GetWorkerOrders(userId, userRolesList));
            return View(workListItemsToDisplay.OrderByDescending(wl => wl.PriorityScore));

        }

        private IEnumerable<IWorkListItem> GetWorkerOrders(string userId, List<string> userRolesList)
        {
            IEnumerable<IWorkListItem> claimableWorkOrder = db.WorkOrders.Where(
                wo => wo.WorkOrderStatus != WorkOrderStatus.Approved)
                .ToList()
                .Where(wo => userRolesList.Any(ur => wo.RolesWhichCanClaim.Contains(ur)));

            IEnumerable<IWorkListItem> workOdersIAmWorkingOn = db.WorkOrders.Where(
                wo => wo.CurrentWorkerId == userId);

            return claimableWorkOrder.Concat(workOdersIAmWorkingOn);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}