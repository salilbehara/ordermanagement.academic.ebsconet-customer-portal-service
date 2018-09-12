using Newtonsoft.Json;

namespace ebsco.svc.webapi.framework.Responses
{
    public class ApiResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        public int StatusCode { get; set; }

        public ApiResponse(int statusCode, string message = null)
        {
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            StatusCode = statusCode;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 200:
                    return "Operation successful.";
                case 201:
                    return "Request fulfilled. New resource created.";
                case 304:
                    return "Resource has not been modified.";
                case 400:
                    return "Invalid request. Please try again later.";
                case 401:
                    return "Authentication necessary to access this resource.";
                case 403:
                    return "Invalid request. Restricted resource.";
                case 404:
                    return "Resource could not be found. Please try again later.";
                case 409: 
                    return "The resource being created already exists.";
                case 500:
                    return "An error occurred. Please try again later.";
                default:
                    return null;
            }
        }
    }
}