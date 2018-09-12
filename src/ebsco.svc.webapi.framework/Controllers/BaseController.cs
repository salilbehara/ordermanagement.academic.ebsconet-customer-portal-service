using ebsco.svc.webapi.framework.Responses;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ebsco.svc.webapi.framework.Controllers
{
    public class BaseController : Controller
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult Ok()
        {
            return base.Ok(new ApiOkResponse());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult Ok(object value)
        {
            return base.Ok(new ApiOkResponse(value));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult StatusCode(int statusCode)
        {
            return base.StatusCode(statusCode, new ApiResponse(statusCode));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult StatusCode(int statusCode, string message)
        {
            return base.StatusCode(statusCode, new ApiResponse(statusCode, message));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult NotFound()
        {
            return base.NotFound(new ApiNotFoundResponse());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult NotFound(string value)
        {
            return base.NotFound(new ApiNotFoundResponse(value));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult Created(string uri, object value)
        {
            return base.Created(uri, new ApiCreatedResponse(uri, value));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult CreatedAtRoute(string routeName, object routeValues, object value)
        {
            var uri = Url.Link(routeName, routeValues);

            return base.Created(uri, value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult Unauthorized()
        {
            return base.Unauthorized();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult BadRequest()
        {
            throw new InvalidOperationException("Use BadRequest(string message) overload instead");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult BadRequest(string message)
        {
            return base.BadRequest(new ApiBadRequestResponse(message));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult BadRequest(Exception exception)
        {
            return base.BadRequest(new ApiBadRequestResponse(exception));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Conflict(string message)
        {
            return base.StatusCode(409, new ApiResponse(409, message));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new IActionResult NoContent()
        {
            return base.StatusCode(204, new ApiResponse(204));
        }
    }
}
