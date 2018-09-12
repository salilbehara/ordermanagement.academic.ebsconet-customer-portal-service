using ebsco.svc.webapi.framework.Models;

namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemDetailModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
        public string TranslationKey { get; set; }
    }
}
