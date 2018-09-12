using ebsco.svc.webapi.framework.Models;
using System;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemHeaderModel : LinkedResourceBaseModel
    {
        public int WorkItemId { get; set; }
        public string WorkItemTypeDescription { get; set; }
        public string TitleName { get; set; }
        public string EbscoOrderNumber { get; set; }
        public string WorkItemStatusDescription { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Owner { get; set; }
    }
}
