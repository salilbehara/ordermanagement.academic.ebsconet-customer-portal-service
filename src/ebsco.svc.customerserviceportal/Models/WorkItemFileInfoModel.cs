using System;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemFileInfoModel
    {
        public int WorkItemId { get; set; }
        public WorkItemFileModel[] WorkItemFiles { get; set; }
    }
}
