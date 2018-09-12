
namespace ebsco.svc.webapi.framework.Responses
{
    public class ApiCreatedResponse : ApiResponse, IApiResultResponse
    {
        public object Result { get; }
        public string Location { get; set; }

        public ApiCreatedResponse(string location, object result = null) : base(201)
        {
            Location = location;
            Result = result;
        }
    }
}