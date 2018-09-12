namespace ebsco.svc.webapi.framework.Models
{
    public class LinkModel
    {
        public string Href { get; private set; }
        public string Rel { get; private set; }
        public string Method { get; private set; }

        public LinkModel(string href, string relation, string method)
        {
            Href = href;
            Rel = relation;
            Method = method;
        }
    }
}
