using ebsco.svc.webapi.framework.Models;
using System;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemFileModel : LinkedResourceBaseModel
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeleteFlag { get; set; }
    }
}
