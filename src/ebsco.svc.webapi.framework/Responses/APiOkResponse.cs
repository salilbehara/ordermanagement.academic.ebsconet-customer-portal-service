using Newtonsoft.Json;

namespace ebsco.svc.webapi.framework.Responses
{
    public class ApiOkResponse : ApiResponse, IApiResultResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; }

        public ApiOkResponse(object result = null) : base(200)
        {
            Result = result;
        }
    }
}