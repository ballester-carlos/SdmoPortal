using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace SdmoPortal.Models
{
    public class WorkOrder : IWorkListItem
    {
        public WorkOrder()
        {
            WorkOrderStatus = WorkOrderStatus.Creating;
            Parts = new List<Part>();
            Labors = new List<Labor>();
        }

        public int WorkOrderId { get; set; }
        [Display(Name = "Customer")]
        [Required(ErrorMessage = "You must choose a customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        [Display(Name = "Order Date")]
        public DateTime OrderDateTime { get; set; }
        [Display(Name = "Target Date")]
        public DateTime? TargetDateTime { get; set; }
        [Display(Name = "Drop Dead Date")]
        public DateTime? DropDeadDateTime { get; set; }
        [StringLength(256,ErrorMessage ="The description must be 256 characters or shorter")]
        public string Description { get; set; }
        [Display(Name = "Status")]
        public WorkOrderStatus WorkOrderStatus { get; set; }
        public decimal Total { get; set; }
        [Display(Name = "Cert. Req.'s")]
        [StringLength(120,ErrorMessage = "The Certification Requirements must be 120 characters or shorter")]
        public string CertificationRequirements { get; set; }
        //TO DO: Integrate ApplicationUser with the rest of the model
        public virtual ApplicationUser CurrentWorker { get; set; }
        public string CurrentWorkerId { get; set; }

        //TODO: Added for Promotions rule to check Parts and Labors Count
        public virtual List<Part> Parts { get; set; }
        public virtual List<Labor> Labors { get; set; }
        [Display(Name = "Rework Notes")]
        [StringLength(256, ErrorMessage = "Rework Notes must be 256 characters or fewer.")]
        public string ReworkNotes { get; set; }

        public int Id
        {
            get
            {
                return WorkOrderId;
            }
        }

        public string Status
        {
            get
            {
                return WorkOrderStatus.ToString();
            }
        }

        public string CurrentWorkerName
        {
            get
            {
                if (CurrentWorker == null)
                    return String.Empty;

                return CurrentWorker.FullName;
            }
        }

        public string EntityFamiliarName
        {
            get{ return "Work Order"; }
        }

        public string EntityFamiliarNamePlural
        {
            get { return "Work Orders"; }
        }

        public string EntityFormalName
        {
            get { return "WorkOrder"; }
        }

        public string EntityFormalNamePlural
        {
            get { return "WorkOrders"; }
        }

        public int PriorityScore
        {
            get { return 0; }
        }

        public IEnumerable<string> RolesWhichCanClaim
        {
            get
            {
                List<string> rolesWhichCanClaim = new List<string>();

                switch(WorkOrderStatus)
                {
                    case WorkOrderStatus.Created:
                        rolesWhichCanClaim.Add("Clerk");
                        rolesWhichCanClaim.Add("Manager");
                        break;
                    case WorkOrderStatus.Processed:
                        rolesWhichCanClaim.Add("Manager");
                        rolesWhichCanClaim.Add("Admin");
                        break;
                    case WorkOrderStatus.Certified:
                        rolesWhichCanClaim.Add("Admin");
                        break;
                    case WorkOrderStatus.Rejected:
                        rolesWhichCanClaim.Add("Manager");
                        rolesWhichCanClaim.Add("Admin");
                        break;
                }
                return rolesWhichCanClaim;
            }
        }

        public PromotionResult ClaimWorkListItem (string userId)
        {
            PromotionResult promotionResult = new PromotionResult();

            //if(!promotionResult.Success)
            //{
            //    return promotionResult;
            //}

            switch (WorkOrderStatus)
            {
                case WorkOrderStatus.Rejected:
                    promotionResult = PromoteToProcessing();
                    break;
                case WorkOrderStatus.Created:
                    promotionResult = PromoteToProcessing();
                    break;
                case WorkOrderStatus.Processed:
                    promotionResult = PromoteToCertifying();
                    break;
                case WorkOrderStatus.Certified:
                    promotionResult = PromoteToApproving();
                    break;
            }

            if (promotionResult.Success)
            {
                CurrentWorkerId = userId;
            }

            return promotionResult;
        }

        public PromotionResult PromoteWorkListItem(string command)
        {
            PromotionResult promotionResult = new PromotionResult();

            switch (command)
            {
                case "PromoteToCreated":
                    promotionResult = PromoteToCreated();
                    break;
                case "PromoteToProcessed":
                    promotionResult = PromoteToProcessed();
                    break;
                case "PromoteToCertified":
                    promotionResult = PromoteToCertified();
                    break;
                case "PromoteToApproved":
                    promotionResult = PromoteToApproved();
                    break;
                case "DemoteToCreated":
                    promotionResult = DemoteToCreated();
                    break;
                case "DemoteToRejected":
                    promotionResult = DemoteToRejected();
                    break;
                case "DemoteToCanceled":
                    promotionResult = DemoteToCanceled();
                    break;
            }

            Log4NetHelper.Log(promotionResult.Message, LogLevel.INFO, "WorkOrders", WorkOrderId, HttpContext.Current.User.Identity.Name, null);

            if (promotionResult.Success)
            {
                CurrentWorker = null;
                CurrentWorkerId = null;

                // Attempt to auto-promotion from Certified To Approved
                if(WorkOrderStatus == WorkOrderStatus.Certified && Parts.Sum(p => p.ExtendedPrice) + Labors.Sum(l => l.ExtendedPrice) < 5000)
                {
                    PromotionResult autoPromotionResult = PromoteToApproved();
                    if(autoPromotionResult.Success)
                    {
                        promotionResult = autoPromotionResult;
                        promotionResult.Message = "AUTOMATIC PROMOTION: " + promotionResult.Message;
                    }
                }
            }

            return promotionResult;
        }

        private PromotionResult PromoteToCreated()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if (WorkOrderStatus != WorkOrderStatus.Creating)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Created status because its current status prevented it.";
            }
            else if (String.IsNullOrWhiteSpace(TargetDateTime.ToString()) ||
                     String.IsNullOrWhiteSpace(DropDeadDateTime.ToString()) ||
                     String.IsNullOrWhiteSpace(Description) )
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Created status because it requires a Target Date, Drop Dead Time and Description.";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Created;
                promotionResult.Message = String.Format("Work order {0} successfuly promoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }

            return promotionResult;
        }

        private PromotionResult PromoteToProcessing()
        {
            if (WorkOrderStatus == WorkOrderStatus.Created || WorkOrderStatus == WorkOrderStatus.Rejected)
            {
                WorkOrderStatus = WorkOrderStatus.Processing;
            }
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = WorkOrderStatus == WorkOrderStatus.Processing;

            if (promotionResult.Success)
            {
                promotionResult.Message = String.Format("Work order {0} successfuly claimed by {1} and promoted to status {2}.", 
                    WorkOrderId,
                    HttpContext.Current.User.Identity.Name,
                    WorkOrderStatus);
            }
            else
            {
                promotionResult.Message = "Failed to promote the work order to Processing status because its current status prevented it.";
            }

            return promotionResult;
        }

        private PromotionResult PromoteToCertifying()
        {
            if (WorkOrderStatus == WorkOrderStatus.Processed)
            {
                WorkOrderStatus = WorkOrderStatus.Certifying;
            }

            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = WorkOrderStatus == WorkOrderStatus.Certifying; ;

            if (promotionResult.Success)
            {
                promotionResult.Message = String.Format("Work order {0} successfuly claimed by {1} and promoted to status {2}.",
                    WorkOrderId,
                    HttpContext.Current.User.Identity.Name,
                    WorkOrderStatus);
            }
            else
            {
                promotionResult.Message = "Failed to promote the work order to Certifying status because its current status prevented it.";
            }

            return promotionResult;
        }

        private PromotionResult PromoteToCertified()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if (WorkOrderStatus != WorkOrderStatus.Certifying)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Certified status because its current status prevented it.";
            }
            if(String.IsNullOrWhiteSpace(CertificationRequirements))
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Certified status because Certification Requirements were not present.";
            }
            else if (Parts.Count == 0 || Labors.Count == 0)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Certified status because it did not contain at least one Part and labor.";
            }
            else if (Parts.Count(p => p.IsInstalled == false) > 0 || Labors.Count(l => l.PercentComplete < 100) > 0)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Certified status because not all Parts have been installed and ...";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Certified;
                promotionResult.Message = String.Format("Work order {0} successfuly promoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }
            return promotionResult;
        }

        private PromotionResult PromoteToProcessed()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if (WorkOrderStatus != WorkOrderStatus.Processing)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Processed status because its current status prevented it.";
            }
            else if(Parts.Count == 0 || Labors.Count == 0)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Processed status because it did not contain at least one Part and labor.";
            }
            else if(String.IsNullOrWhiteSpace(Description))
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Processed status because it requires Description.";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Processed;
                promotionResult.Message = String.Format("Work order {0} successfuly promoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }

            return promotionResult;
        }

        private PromotionResult PromoteToApproving()
        {
            if (WorkOrderStatus == WorkOrderStatus.Certified)
            {
                WorkOrderStatus = WorkOrderStatus.Approving;
            }
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = WorkOrderStatus == WorkOrderStatus.Approving;

            if (promotionResult.Success)
            {
                promotionResult.Message = String.Format("Work order {0} successfuly claimed by {1} and promoted to status {2}.",
                    WorkOrderId,
                    HttpContext.Current.User.Identity.Name,
                    WorkOrderStatus);
            }
            else
            {
                promotionResult.Message = "Failed to promote the work order to Approving status because its current status prevented it.";
            }

            return promotionResult;
        }

        private PromotionResult PromoteToApproved()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if (WorkOrderStatus != WorkOrderStatus.Approving && WorkOrderStatus != WorkOrderStatus.Certified)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to promote the work order to Approved status because its current status prevented it.";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Approved;
                promotionResult.Message = String.Format("Work order {0} successfuly promoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }

            return promotionResult;
        }

        private PromotionResult DemoteToCreated()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if(WorkOrderStatus != WorkOrderStatus.Approving)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to demote the work order to Created status because its current status prevented it.";
            }

            if (String.IsNullOrWhiteSpace(ReworkNotes))
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to demote the work order to Created status because Rework Notes must be present.";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Created;
                promotionResult.Message = String.Format("Work order {0} successfuly demoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }

            return promotionResult;
        }

        private PromotionResult DemoteToCanceled()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if (WorkOrderStatus != WorkOrderStatus.Approving)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to demote the work order to Cancel status because its current status prevented it.";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Canceled;
                promotionResult.Message = String.Format("Work order {0} successfuly demoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }

            return promotionResult;
        }

        private PromotionResult DemoteToRejected()
        {
            PromotionResult promotionResult = new PromotionResult();
            promotionResult.Success = true;

            if (WorkOrderStatus != WorkOrderStatus.Approving)
            {
                promotionResult.Success = false;
                promotionResult.Message = "Failed to demote the work order to Rejected status because its current status prevented it.";
            }

            if (promotionResult.Success)
            {
                WorkOrderStatus = WorkOrderStatus.Rejected;
                promotionResult.Message = String.Format("Work order {0} successfuly demoted to status {1}.", WorkOrderId, WorkOrderStatus);
            }

            return promotionResult;
        }

        public PromotionResult RelinquishWorkListItem()
        {
            throw new NotImplementedException();
        }
    }

    public enum WorkOrderStatus
    {
        Creating = 5,
        Created = 10,
        Processing = 15,
        Processed = 20,
        Certifying = 25,
        Certified = 30,
        Approving = 35,
        Approved = 40,
        Rejected = -10,
        Canceled = -20
    }
}