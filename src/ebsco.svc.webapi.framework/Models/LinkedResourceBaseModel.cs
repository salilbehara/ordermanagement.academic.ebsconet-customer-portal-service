using System.Collections.Generic;

namespace ebsco.svc.webapi.framework.Models
{
    public class LinkedResourceBaseModel
    {
        public List<LinkModel> Links { get; set; }

        public LinkedResourceBaseModel()
        {
            Links = new List<LinkModel>();
        }
    }
}
