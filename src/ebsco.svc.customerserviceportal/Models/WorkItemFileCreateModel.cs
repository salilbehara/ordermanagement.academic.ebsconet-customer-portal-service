using System.IO;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemFileCreateModel
    {
        public string WorkItemFileName { get; set; }
        public byte[] WorkItemFileContent { get; set; }
    }
}
