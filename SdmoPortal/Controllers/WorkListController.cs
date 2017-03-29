using System.Web.Mvc;
using SdmoPortal.DataLayer;

namespace SdmoPortal.Controllers
{
    [Authorize]
    public class WorkListController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: WorkList
        public ActionResult Index()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}