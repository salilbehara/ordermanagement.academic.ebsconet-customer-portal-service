namespace ebsco.svc.customerserviceportal.Models
{
    public class ApplicationSettings
    {
        public virtual string FeatureFlag_Url { get; set; }
        public virtual string FeatureFlag_Prefix { get; set; }
        public virtual string FeatureFlag_Tenant { get; set; }
        public virtual int FeatureFlag_CacheTime { get; set; }
        public virtual string WorkItemTrackerService_Url { get; set; }
        public virtual string MediaServerService_URL { get; set; }
        public virtual string EBSCONET_JWT_Secret { get; set; }
    }
}
