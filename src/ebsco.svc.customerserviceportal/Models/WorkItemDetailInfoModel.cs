namespace ebsco.svc.customerserviceportal.Models
{
    public class WorkItemDetailInfoModel
    {
        public int WorkItemId { get; set; }
        public WorkItemDetailModel[] WorkItemDetails { get; set; }
        public string TranslatedClaimDetailLink { get; set; }
    }
}
