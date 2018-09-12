using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ebsco.svc.webapi.framework.Responses
{
    public class ApiNotFoundResponse : ApiResponse, IApiResultResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object Result { get; }

        public ApiNotFoundResponse(string message = null) : base(StatusCodes.Status404NotFound, message)
        {

        }
    }
}