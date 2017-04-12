using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SdmoPortal.Models
{
    public class Widget : IWorkListItem
    {
        public Widget()
        {
            WidgetStatus = WidgetStatus.Created;
        }

        public int WidgetId { get; set; }

        [Required(ErrorMessage = "You must enter a description.")]
        [StringLength(256, ErrorMessage = "The description must be 256 characters or shorter.")]
        public string Description { get; set; }

        [Display(Name = "Main Bus Code")]
        [StringLength(12, ErrorMessage = "Main Bus Code must be 12 characters or shorter.")]
        public string MainBusCode { get; set; }

        [Display(Name = "Test Pass Date")]
        public DateTime? TestPassDateTime { get; set; }

        [Display(Name = "Status")]
        public WidgetStatus WidgetStatus { get; set; }

        public virtual ApplicationUser CurrentWorker { get; set; }

        public string CurrentWorkId { get; set; }

        public int Id
        {
            get { return WidgetId; }
        }

        public string Status
        {
            get { return WidgetStatus.ToString(); }
        }

        public string CurrentWorkerName
        {
            get
            {
                if(CurrentWorker == null)
                    return String.Empty;
                return CurrentWorker.FullName;
            }
        }

        public string  EntityFamiliarName {
            get { return "Widget"; } 
        }

        public string EntityFamiliarNamePlural
        {
            get { return "Widgets"; }
        }

        public string EntityFormalName
        {
            get { return "Widget"; }
        }

        public string EntityFormalNamePlural
        {
            get { return "Widgets"; }
        }

        public int PriorityScore
        {
            get
            {
                int priorityScore = (int)WidgetStatus;
                priorityScore += Description.Length / 10;

                if (priorityScore < 0) priorityScore = 0;
                if (priorityScore > 100) priorityScore = 100;

                return priorityScore;
            }
        }

        public IEnumerable<string> RolesWhichCanClaim
        {
            get
            {
                List<string> rolesWhichCanClaim = new List<string>();

                switch (WidgetStatus)
                {
                    case WidgetStatus.Created:
                        rolesWhichCanClaim.Add("Manager");
                        rolesWhichCanClaim.Add("Admin");
                        break;
                    case WidgetStatus.Integrated:
                        rolesWhichCanClaim.Add("Admin");
                        break;
                }
                return rolesWhichCanClaim;
            }
        }

        public PromotionResult ClaimWorkListItem(string userId)
        {
            PromotionResult promotionResult = WorkListBusinessRules.CanClaimWorkListItem(userId);

            if (!promotionResult.Success)
            {
                Log4NetHelper.Log(promotionResult.Message, LogLevel.WARN, EntityFormalNamePlural, WidgetId, HttpContext.Current.User.Identity.Name, null);
                return promotionResult;
            }

            switch (WidgetStatus)
            {
                case WidgetStatus.Created:
                    promotionResult = PromoteToIntegrating();
                    break;
                case WidgetStatus.Integrated:
                    promotionResult = PromoteToApproving();
                    break;
            }

            if (promotionResult.Success)
                CurrentWorkerId = userId;

            Log4NetHelper.Log(promotionResult.Message, LogLevel.INFO, EntityFormalNamePlural, WidgetId, HttpContext.Current.User.Identity.Name, null);
            return promotionResult;
        }

    }
}