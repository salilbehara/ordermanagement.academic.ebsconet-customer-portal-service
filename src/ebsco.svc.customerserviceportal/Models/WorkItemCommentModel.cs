using System;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemCommentModel
    {
        public int WorkItemId { get; set; }
        public int CommentId { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Details { get; set; }
    }
}
