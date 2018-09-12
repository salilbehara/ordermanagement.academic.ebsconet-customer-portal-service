using ebsco.svc.webapi.framework.Models;

namespace ebsco.svc.webapi.framework.Helpers
{
    public class ResourceWrapper<TModel> : LinkedResourceBaseModel
        where TModel : class
    {
        public TModel Value { get; set; }

        public ResourceWrapper(TModel value)
        {
            Value = value;
        }
    }
}
