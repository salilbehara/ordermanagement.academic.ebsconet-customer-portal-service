using ebsco.svc.webapi.framework.Models;
using System;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemCommentCreateModel
    {
        public int? CommentMethodId { get; set; }
        public int? CommentFromContactTypeId { get; set; }
        public int? CommentToContactTypeId {get; set; }
        public string Organization {get; set; }
        public string Details { get; set; }
        public bool IsInComment {get; set; }
    }
}
